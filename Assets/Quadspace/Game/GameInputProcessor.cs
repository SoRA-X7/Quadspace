using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Quadspace.Game.Moves;
using UnityEngine;

namespace Quadspace.Game {
    public class GameInputProcessor {
        private FieldBehaviour fb;
        
        private readonly Queue<Instruction> instructions = new Queue<Instruction>();
        private Instruction? prevInstruction;

        public GameInputProcessor(FieldBehaviour fb) {
            this.fb = fb;
        }

        public async UniTask MoveAsync(IEnumerable<Instruction> input, bool hold, CancellationToken cancel) {
            var sb = new StringBuilder();
            if (hold) sb.Append("Hold ");
            instructions.Clear();
            foreach (var i in input) {
                instructions.Enqueue(i);
                sb.Append(i.ToString()).Append(' ');
            }
            Debug.Log(sb.ToString());

            var hypertap = MatchEnvironment.config.Delays["hypertap"];

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

            if (hold) {
                fb.Hold();
                await Wait(hypertap);
            }

            while (instructions.Count != 0) {
                var inst = instructions.Dequeue();
                if (inst == prevInstruction) {
                    await Wait(hypertap);
                }
                if (cancel.IsCancellationRequested) return;
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
                            await Wait(MatchEnvironment.config.Delays["softdrop"]);
                        }

                        break;
                }

                prevInstruction = inst;
                await Wait(hypertap);
            }

            await fb.LockPiece();

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

        private async UniTask Wait(int frames) {
            if (frames > 0) {
                await UniTask.DelayFrame(frames, PlayerLoopTiming.FixedUpdate);
            }
        }
    }
}