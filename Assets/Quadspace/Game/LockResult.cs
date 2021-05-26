using System.Collections.Generic;

namespace Quadspace.Game {
    public struct LockResult {
        public List<int> clearedLines;
        public PlacementKind kind;
        public bool pc;
    }
}