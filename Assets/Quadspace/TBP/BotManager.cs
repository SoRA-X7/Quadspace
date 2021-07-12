using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Quadspace.Game;
using Quadspace.Game.Moves;
using Quadspace.TBP.Messages;
using Quadspace.TBP.Randomizer;
using UnityEngine;
using Utf8Json;
using Debug = UnityEngine.Debug;
using Path = Quadspace.Game.Moves.Path;

namespace Quadspace.TBP {
    public class BotManager : IDisposable {
        private string pathToExecutable;
        private Process process;
        public BotStatus Status { get; private set; } = BotStatus.NotInitialized;
        private StreamReader stdout;
        private StreamReader stderr;
        private StreamWriter stdin;
        private UniTask runner;
        private CancellationTokenSource cancelSource = new CancellationTokenSource();
        private SemaphoreSlim semaphore;
        private GameInputProcessor controller;
        private FieldBehaviour fb;
        private Field field;
        private Field foreseeField;

        private MoveGenerator moves;
        private Path? pickedPath;
        private bool forfeit;
        private readonly Stopwatch suggestionTimer = new Stopwatch();
        public TbpInfoMessage BotInfo { get; private set; }
        public int PickedMoveIndex { get; private set; }
        public long ResponseTimeInMillisecond { get; private set; }

        private string logPathDir;

        public BotManager(string pathToExecutable, GameInputProcessor controller) {
            this.pathToExecutable = pathToExecutable;
            this.controller = controller;
            semaphore = new SemaphoreSlim(1, 1);
            logPathDir = System.IO.Path.GetDirectoryName(Application.consoleLogPath);
        }

        public void Launch(FieldBehaviour fb) {
            this.fb = fb;
            field = fb.field;
            foreseeField = field.Clone();
            fb.NewPiece += OnNewPiece;
            fb.PieceSpawned += OnPieceSpawned;
            var start = new ProcessStartInfo {
                FileName = pathToExecutable,
                WorkingDirectory = System.IO.Path.GetDirectoryName(pathToExecutable),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            process = Process.Start(start);
            Status = BotStatus.Initializing;
            stdout = process.StandardOutput;
            stdin = process.StandardInput;
            stderr = process.StandardError;
            runner = Run();
        }

        private async void OnNewPiece(int p) {
            if (Status < BotStatus.Running) {
                await UniTask.WaitUntil(() => Status == BotStatus.Running,
                    cancellationToken: fb.CancelTokenSource.Token);
            } else if (Status > BotStatus.Running) {
                return;
            }

            await Send(new TbpNewPieceMessage(MatchEnvironment.pieceRegistry[p].name));
        }

        private async void OnPieceSpawned(Piece p) {
            if (Status < BotStatus.Running) {
                await UniTask.WaitUntil(() => Status == BotStatus.Running,
                    cancellationToken: fb.CancelTokenSource.Token);
            } else if (Status > BotStatus.Running) {
                return;
            }

            await Send(new TbpFrontendMessage(FrontendMessageType.suggest));
            suggestionTimer.Restart();
            var unhold = field.Hold ?? field.Next.First();
            moves = await MoveGenerator.Run(field, p,
                p.kind != unhold ? new Piece(unhold, 4, 19, 0, SpinStatus.None) : (Piece?) null);
        }

        private async UniTask Run() {
            var tbpStartMessage = new TbpStartMessage {
                board = field.Rows.Take(40).Select(r => r.blocks.Select(b => b ? b.blockID : null).ToArray()).ToList(),
                back_to_back = field.BackToBack,
                combo = field.Ren + 1,
                hold = field.Hold != null ? MatchEnvironment.blockRegistry[(int) field.Hold].blockID : null,
                queue = field.Next.Select(i => MatchEnvironment.pieceRegistry[i].name).ToList(),
                randomizer = new SevenBagRandomizerStart(new List<string>(fb.Bag))
            };

            await UniTask.SwitchToThreadPool();
            {
                var line = await stdout.ReadLineAsync();
                LogBotMessage(line);
                if (line == null) return;

                var msg = JsonSerializer.Deserialize<TbpBotMessage>(line);
                if (msg.type != BotMessageType.info) {
                    Debug.LogError("Killed bot because received message was unexpected. " +
                                   $"Expected: {nameof(BotMessageType.info)}, Actual: {msg.type}");
                    return;
                }

                BotInfo = JsonSerializer.Deserialize<TbpInfoMessage>(line);
                await Send(new TbpRulesMessage());
            }
            {
                var line = await stdout.ReadLineAsync();
                LogBotMessage(line);
                if (line == null) return;

                var msg = JsonSerializer.Deserialize<TbpBotMessage>(line);
                if (msg.type != BotMessageType.ready) {
                    Debug.LogError("Killed bot because received message was unexpected. " +
                                   $"Expected: {nameof(BotMessageType.ready)}, Actual: {msg.type}");
                    return;
                }

                Status = BotStatus.Ready;
            }

            await Send(tbpStartMessage);
            Status = BotStatus.Running;

            try {
                while (true) {
                    var line = await stdout.ReadLineAsync();
                    LogBotMessage(line);
                    if (line == null) return;

                    var msg = JsonSerializer.Deserialize<TbpBotMessage>(line);
                    if (msg.type == BotMessageType.suggestion) {
                        ResponseTimeInMillisecond = suggestionTimer.ElapsedMilliseconds;
                        if (moves == null) {
                            await UniTask.WaitWhile(() => moves == null, PlayerLoopTiming.FixedUpdate);
                        }

                        var suggestion = JsonSerializer.Deserialize<TbpSuggestionMessage>(line);
                        var success = false;
                        PickedMoveIndex = 0;
                        foreach (var candidate in suggestion.moves.Select(c => (Piece) c)) {
                            if (candidate.GetCanonicals().Any(can => moves.locked.ContainsKey(can))) {
                                await UniTask.WhenAll(
                                    Send(new TbpPlayMessage(candidate)).AsAsyncUnitUniTask(),
                                    UniTask.Run(() => pickedPath = moves.RebuildPath(candidate,
                                        candidate.kind != fb.currentPiece.content.kind)));
                                moves = null;
                                success = true;
                                UniTask.Run(() => controller.MoveAsync(pickedPath!.Value.instructions,
                                        candidate.kind != fb.currentPiece.content.kind, fb.CancelTokenSource.Token))
                                    .Forget();
                                break;
                            }

                            PickedMoveIndex++;
                        }

                        if (!success) {
                            forfeit = true;
                            return;
                        }
                    } else {
                        Debug.LogError("Killed bot because received message was unexpected. " +
                                       $"Expected: {nameof(BotMessageType.suggestion)}, Actual: {msg.type}");
                        return;
                    }
                }
            } finally {
                await Quit();
            }
        }

        private async UniTask Quit() {
            if (!process.HasExited) {
                await Send(new TbpFrontendMessage(FrontendMessageType.quit));
            }

            var err = stderr.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(err)) {
                var botName = BotInfo.name;
                if (string.IsNullOrWhiteSpace(botName)) {
                    botName = System.IO.Path.GetFileName(pathToExecutable);
                }

                var path = System.IO.Path.Combine(logPathDir,
                    $"Error_{botName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                Debug.LogError(err);
                File.WriteAllText(path, err);
                Debug.Log($"Error log is created at {path}");
                Status = BotStatus.Error;
            } else {
                Status = BotStatus.Quit;
            }
        }

        private static void LogBotMessage(string line) {
            if (MatchEnvironment.config.Logging["logBotMessage"] == 1) {
                Debug.Log(line);
            }
        }

        private async UniTask Send<T>(T msg) where T : TbpFrontendMessage {
            var json = JsonSerializer.ToJsonString(msg);
            if (MatchEnvironment.config.Logging["logFrontendMessage"] == 1) {
                Debug.Log(json);
            }

            try {
                await semaphore.WaitAsync().AsUniTask(false);
                await stdin.WriteLineAsync(json);
            } finally {
                semaphore.Release();
            }
        }

        public void Dispose() {
            cancelSource.Cancel();
            if (Status == BotStatus.NotInitialized) return;
            fb.NewPiece -= OnNewPiece;
            fb.PieceSpawned -= OnPieceSpawned;

            if (Status >= BotStatus.Error) return;
            
            Quit().AsTask().Wait();
        }
    }
}