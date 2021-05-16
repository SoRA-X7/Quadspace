using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quadspace.Game {
    [CreateAssetMenu]
    public class RotationSystemModuleDescriptor : ScriptableObject {
        public List<string> apply;
        public List<Vector2Int> offsetsN;
        public List<Vector2Int> offsetsE;
        public List<Vector2Int> offsetsS;
        public List<Vector2Int> offsetsW;

        public List<Vector2Int> this[int r] => r switch {
            0 => offsetsN,
            1 => offsetsE,
            2 => offsetsS,
            3 => offsetsW,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}