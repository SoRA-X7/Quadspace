using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Quadspace.Game;
using Quadspace.Game.Moves;
using SFB;
using TMPro;
using UnityEngine;

namespace Quadspace.Quadspace.TBP {
    public class BotController : MonoBehaviour {
        private readonly Queue<Instruction> instructions = new Queue<Instruction>();
        private Instruction? prevInstruction;
        private FieldBehaviour fb;
        public BotManager manager;

        private string botExePath;
        [SerializeField] private TMP_Text botExePathText;

        private void Start() {
            fb = GetComponent<FieldBehaviour>();

            fb.Initialized += () => {
                manager?.Launch(fb);
            };
        }

        public void SelectBotExe() {
            botExePath = null;
            botExePathText.text = "Not Set";
            manager?.Dispose();
            manager = null;
            var paths = StandaloneFileBrowser.OpenFilePanel("Choose your bot executable", "", "", false);
            if (paths.Length != 1) return;
            botExePath = paths[0];
            manager = new BotManager(botExePath, this);
            botExePathText.text = System.IO.Path.GetFileName(botExePath);
        }

        private void OnDestroy() {
            manager?.Dispose();
        }

        public async UniTask MoveAsync(IEnumerable<Instruction> input, bool hold) {
            var sb = new StringBuilder();
            if (hold) sb.Append("Hold ");
            foreach (var i in input) {
                instructions.Enqueue(i);
                sb.Append(i.ToString()).Append(' ');
            }
            Debug.Log(sb.ToString());

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

            if (hold) {
                fb.Hold();
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }

            while (instructions.Count != 0) {
                var inst = instructions.Dequeue();
                if (inst == prevInstruction) await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                switch (inst) {
                    case Instruction.Left:
                        Strafe(-1);
                        break;
                    case Instruction.Right:
                        Strafe(1);
                        break;
                    case Instruction.Cw:
                        Rotate(true);
                        break;
                    case Instruction.Ccw:
                        Rotate(false);
                        break;
                    case Instruction.SonicDrop:
                        while (true) {
                            var down = fb.currentPiece.content.Strafe(0, -1);
                            if (fb.field.Collides(down)) break;
                            fb.currentPiece.Set(down);
                            await UniTask.DelayFrame(2, PlayerLoopTiming.FixedUpdate);
                        }

                        break;
                }

                prevInstruction = inst;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }

            fb.LockPiece();

            prevInstruction = null;
        }

        private void Strafe(int x) {
            var moved = fb.currentPiece.content.Strafe(x, 0);
            if (fb.field.Collides(moved)) throw new Exception();
            fb.currentPiece.Set(moved);
        }

        private void Rotate(bool cw) {
            var rotated = fb.field.Rotate(fb.currentPiece.content, cw);
            if (rotated == null) throw new Exception();
            fb.currentPiece.Set(rotated.Value);
        }
    }
}