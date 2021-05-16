using System;
using UnityEngine;

namespace Quadspace.Game.ScriptableObjects {
    [CreateAssetMenu]
    public class BlockDescriptor : IndexedScriptableObject {
        public string blockID;
        public Color color;
        public Material material;
    }
}