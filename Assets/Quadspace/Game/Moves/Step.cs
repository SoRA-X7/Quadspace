using System;

namespace Quadspace.Game.Moves {
    public class Step : IComparable<Step> {
        public readonly Step parent;
        public readonly int cost;
        public readonly int depth;
        public readonly Piece piece;
        public readonly Instruction inst;

        public Step(Step parent, int cost, int depth, Piece piece, Instruction inst) {
            this.parent = parent;
            this.cost = cost;
            this.depth = depth;
            this.piece = piece;
            this.inst = inst;
        }

        public int CompareTo(Step other) {
            var costComparison = cost.CompareTo(other.cost);
            if (costComparison != 0) return costComparison * -1;
            return depth.CompareTo(other.depth) * -1;
        }
    }
}