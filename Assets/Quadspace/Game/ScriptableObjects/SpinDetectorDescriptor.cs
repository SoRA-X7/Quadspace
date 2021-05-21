using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quadspace.Game.ScriptableObjects {
    [CreateAssetMenu]
    public class SpinDetectorDescriptor : IndexedScriptableObject {
        public new string name;
        public string applyPiece;
        public List<int> forceFullKicks;
        public Vector2Int[] miniPattern;
        public Vector2Int[] nonMiniPattern;

        public SpinStatus CheckSpin(Field field, Piece piece, int kick) {
            var miniChecks = 0;
            foreach (var pos in GetPositions(miniPattern, piece)) {
                if (field.Occupied(pos)) miniChecks++;
            }

            var nonMiniChecks = 0;
            foreach (var pos in GetPositions(nonMiniPattern, piece)) {
                if (field.Occupied(pos)) nonMiniChecks++;
            }

            if (miniChecks + nonMiniChecks >= 3) {
                if (forceFullKicks.Contains(kick) || miniChecks == 2) {
                    return SpinStatus.Full;
                } else {
                    return SpinStatus.Mini;
                }
            } else {
                return SpinStatus.None;
            }
        }

        private static IEnumerable<Vector2Int> GetPositions(Vector2Int[] pat, Piece piece) {
            return pat.Select(pos => piece.r switch {
                0 => pos,
                1 => new Vector2Int(pos.y, -pos.x),
                2 => new Vector2Int(-pos.x, -pos.y),
                3 => new Vector2Int(-pos.y, pos.x),
                _ => throw new ArgumentOutOfRangeException()
            }).Select(pos => pos + new Vector2Int(piece.x, piece.y));
        }
    }
}