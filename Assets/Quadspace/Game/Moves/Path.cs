using System.Collections.Generic;

namespace Quadspace.Game.Moves {
    public struct Path {
        public bool hold;
        public IReadOnlyList<Instruction> instructions;
        public Piece result;
        public int time;
    }
}