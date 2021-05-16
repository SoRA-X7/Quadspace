using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Quadspace.Game;
using Quadspace.Game.Moves;
using Quadspace.Game.ScriptableObjects;
using Quadspace.Quadspace.TBP.Messages;
using Utf8Json;
using Debug = UnityEngine.Debug;
using Path = Quadspace.Game.Moves.Path;

namespace Quadspace.Quadspace.TBP {
    public class BotManager : IDisposable {
        private string pathToExecutable;
        private Process process;
        private BotStatus status = BotStatus.NotInitialized;
        private StreamReader stdout;
        private StreamReader stderr;
        private StreamWriter stdin;
        private UniTask runner;
        private BotController controller;
        private FieldBehaviour fb;
        private Field field;

        private MoveGenerator moves;
        private Path? pickedPath;
        private bool forfeit;
        public TbpInfoMessage BotInfo { get; private set; }

        public BotManager(string pathToExecutable, BotController controller) {
            this.pathToExecutable = pathToExecutable;
            this.controller = controller;
        }

        public void Launch(FieldBehaviour fb) {
            this.fb = fb;
            field = fb.field;
            fb.NewPiece += OnNewPiece;
            fb.PieceSpawned += OnPieceSpawned;
            var start = new ProcessStartInfo {
                FileName = pathToExecutable,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            process = Process.Start(start);
            status = BotStatus.Initializing;
            stdout = process.StandardOutput;
            stdin = process.StandardInput;
            stderr = process.StandardError;
            runner = Run();
        }

        private async void OnNewPiece(int p) {
            if (status < BotStatus.Running) {
                await UniTask.WaitUntil(() => status == BotStatus.Running);
            } else if (status > BotStatus.Running) {
                return;
            }

            await Send(new TbpNewPieceMessage(MatchEnvironment.pieceRegistry[p].name));
        }

        private async void OnPieceSpawned(Piece p) {
            if (status < BotStatus.Running) {
                await UniTask.WaitUntil(() => status == BotStatus.Running);
            } else if (status > BotStatus.Running) {
                return;
            }

            await Send(new TbpFrontendMessage(FrontendMessageType.suggest));
            var unhold = field.Hold ?? field.Next.First();
            moves = await MoveGenerator.Run(field, p,
                p.kind != unhold ? new Piece(unhold, 4, 19, 0, SpinStatus.None) : (Piece?) null);
            Debug.Log("Move generated");
        }

        private async UniTask Run() {
            var tbpStartMessage = new TbpStartMessage {
                board = field.Rows.Take(40).Select(r => r.blocks.Select(b => b ? b.blockID : null).ToArray()).ToList(),
                back_to_back = field.BackToBack,
                combo = field.Ren + 1,
                hold = field.Hold != null ? MatchEnvironment.blockRegistry[(int) field.Hold].blockID : null,
                queue = field.Next.Select(i => MatchEnvironment.pieceRegistry[i].name).ToList()
            };
            
            await UniTask.SwitchToThreadPool();
            {
                var line = await stdout.ReadLineAsync();
                Debug.Log(line);
                if (line == null) {
                    status = BotStatus.Quit;
                    return;
                }

                var msg = JsonSerializer.Deserialize<TbpBotMessage>(line);
                if (msg.type != BotMessageType.info) {
                    await ErrorAndQuit();
                    return;
                }

                BotInfo = JsonSerializer.Deserialize<TbpInfoMessage>(line);
                await Send(new TbpRulesMessage());
            }
            {
                var line = await stdout.ReadLineAsync();
                Debug.Log(line);
                if (line == null) {
                    status = BotStatus.Quit;
                    return;
                }

                var msg = JsonSerializer.Deserialize<TbpBotMessage>(line);
                if (msg.type != BotMessageType.ready) {
                    await ErrorAndQuit();
                    return;
                }

                status = BotStatus.Ready;
            }
            
            await Send(tbpStartMessage);
            status = BotStatus.Running;

            while (true) {
                try {
                    var line = await stdout.ReadLineAsync();
                    Debug.Log(line);
                    if (line == null) {
                        Debug.LogError(stderr.ReadLine());
                        status = BotStatus.Quit;
                        return;
                    }

                    var msg = JsonSerializer.Deserialize<TbpBotMessage>(line);
                    if (msg.type == BotMessageType.suggestion) {
                        await UniTask.WaitWhile(() => moves == null);

                        var suggestion = JsonSerializer.Deserialize<TbpSuggestionMessage>(line);
                        var success = false;
                        foreach (var candidate in suggestion.moves.Select(c => (Piece) c)) {
                            if (moves.locked.ContainsKey(candidate)) {
                                await UniTask.WhenAll(
                                    Send(new TbpPlayMessage(candidate)).AsAsyncUnitUniTask(),
                                    UniTask.Run(() => pickedPath = moves.RebuildPath(candidate,
                                        candidate.kind != fb.currentPiece.content.kind)));
                                moves = null;
                                success = true;
                                UniTask.Run(() => controller.MoveAsync(pickedPath!.Value.instructions,
                                    candidate.kind != fb.currentPiece.content.kind)).Forget();
                                break;
                            }
                        }

                        if (!success) {
                            forfeit = true;
                            await ErrorAndQuit();
                            return;
                        }
                    } else {
                        throw new ArgumentOutOfRangeException();
                    }
                } catch (Exception e) {
                    if (!process.HasExited) {
                        await Send(new TbpFrontendMessage(FrontendMessageType.quit));
                    }

                    status = BotStatus.Quit;
                    throw;
                }
            }
        }

        private async UniTask ErrorAndQuit() {
            status = BotStatus.Error;
            await Send(new TbpFrontendMessage(FrontendMessageType.quit));
            status = BotStatus.Quit;
        }

        private async UniTask Send<T>(T msg) where T : TbpFrontendMessage {
            var json = JsonSerializer.ToJsonString(msg);
            Debug.Log(json);
            await stdin.WriteLineAsync(json);
        }

        public void Dispose() {
            if (status > BotStatus.NotInitialized) {
                fb.NewPiece -= OnNewPiece;
                fb.PieceSpawned -= OnPieceSpawned;
            }

            if (process?.HasExited ?? true) return;
            stdin.WriteLine(JsonSerializer.ToJsonString(new TbpFrontendMessage(FrontendMessageType.quit)));
        }
    }
}