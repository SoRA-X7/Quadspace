using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quadspace.Game.ScriptableObjects {
    [CreateAssetMenu][Serializable]
    public class RotationSystemModuleDescriptor : ScriptableObject {
        public List<string> apply;
        public List<Vector2Int> offsetsNE;
        public List<Vector2Int> offsetsNW;
        public List<Vector2Int> offsetsES;
        public List<Vector2Int> offsetsEN;
        public List<Vector2Int> offsetsSW;
        public List<Vector2Int> offsetsSE;
        public List<Vector2Int> offsetsWN;
        public List<Vector2Int> offsetsWS;

        public List<Vector2Int> Get(int r, bool cw) {
            return (r, cw) switch {
                (0, true) => offsetsNE,
                (0, false) => offsetsNW,
                (1, true) => offsetsES,
                (1, false) => offsetsEN,
                (2, true) => offsetsSW,
                (2, false) => offsetsSE,
                (3, true) => offsetsWN,
                (3, false) => offsetsWS,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}