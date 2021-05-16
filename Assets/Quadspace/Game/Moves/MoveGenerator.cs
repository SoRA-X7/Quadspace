using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Quadspace.Game.Moves {
    // From Hikari.AI.Moves.Mirai
    public class MoveGenerator {
        public List<Step> next;
        public Dictionary<Piece, Step> locked;
        public Dictionary<Piece, Step> tree;
        private Field field;

        public static UniTask<MoveGenerator> Run(Field field, Piece spawned, Piece? unhold) {
            return UniTask.RunOnThreadPool(() => {
                var gen = new MoveGenerator(field);
                gen.Generate(spawned);
                if (unhold != null) gen.Generate(unhold.Value);
                return gen;
            });
        }

        private MoveGenerator(in Field field) {
            next = new List<Step>(100);
            locked = new Dictionary<Piece, Step>(100);
            tree = new Dictionary<Piece, Step>(100);
            this.field = field;
        }

        private void Generate(Piece spawned) {
            var root = new Step(null, 0, 0, spawned, Instruction.None);
            tree.Add(spawned, root);
            next.Add(root);

            while (next.Count > 0) {
                next.Sort();
                var origin = next[next.Count - 1];
                next.RemoveAt(next.Count - 1);
                var piece = origin.piece;

                var dropped = field.SonicDrop(piece);
                // if (dropped.IsInvalid) continue;

                if (origin.depth < 16) {
                    Append(origin, piece.Strafe(-1, 0), Instruction.Left);
                    Append(origin, piece.Strafe(1, 0), Instruction.Right);
                    Append(origin, field.Rotate(piece, true), Instruction.Cw);
                    Append(origin, field.Rotate(piece, false), Instruction.Ccw);

                    if (dropped.y != piece.y) {
                        Append(origin, dropped, Instruction.SonicDrop);
                    }
                }

                if (!locked.ContainsKey(dropped)) {
                    locked.Add(dropped, origin);
                }
            }
        }

        private void Append(in Step origin, Piece result, Instruction inst) {
            if (field.Collides(result)) {
                return;
            }

            int t;

            if (inst == Instruction.SonicDrop) {
                t = 2 * (origin.piece.y - result.y);
                // if (result.Kind != PieceKind.T && origin.cost + t >= 20) return;
            } else {
                t = 1;
            }

            if (origin.inst == inst) {
                t += 1;
            }

            var step = new Step(origin, origin.cost + t, origin.depth + 1, result, inst);

            if (!tree.ContainsKey(result) && step.depth < 16) {
                tree.Add(result, step);
                next.Add(step);
            }
        }

        private void Append(in Step origin, Piece? result, Instruction inst) {
            if (result == null) return;
            Append(origin, result.Value, inst);
        }

        public Path? RebuildPath(Piece to, bool holdUsed) {
            var instructions = new List<Instruction>();
            if (!locked.TryGetValue(to, out var leaf)) return null;
            while (leaf.parent != null) {
                instructions.Add(leaf.inst);
                leaf = leaf.parent;
            }

            instructions.Reverse();

            var path = new Path {
                hold = holdUsed,
                instructions = instructions.AsReadOnly(),
                result = to,
                time = locked[to].cost
            };

            return path;
        }
    }
}