using System.Collections.Generic;
using UnityEngine;

namespace Quadspace.Game {
    public struct Piece {
        public int kind;
        public int x;
        public int y;
        public int r;
        public SpinStatus spin;

        public Piece(int kind, int x, int y, int r, SpinStatus spin) {
            this.kind = kind;
            this.x = x;
            this.y = y;
            this.r = r;
            this.spin = spin;
        }

        public readonly IEnumerable<Vector2Int> GetPositions() {
            foreach (var v in MatchEnvironment.pieceRegistry[kind].GetBlocks(r)) {
                yield return v + new Vector2Int(x, y);
            }
        }

        public readonly RotationSystemModuleDescriptor GetRotationSystem(MatchEnvironment env) {
            env.rotationSystemLookup.TryGetValue(MatchEnvironment.pieceRegistry[kind], out var sys);
            return sys;
        }

        public readonly Piece Strafe(int x, int y) {
            return new Piece(kind, this.x + x, this.y + y, r, SpinStatus.None);
        }

        public override string ToString() {
            return $"Piece {kind}@({x}, {y}),{r} ${spin}";
        }
    }
}