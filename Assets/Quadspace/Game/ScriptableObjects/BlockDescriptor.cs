using System;
using UnityEngine;

namespace Quadspace.Game.ScriptableObjects {
    [CreateAssetMenu]
    public class BlockDescriptor : IndexedScriptableObject {
        public string blockID;
        [ColorUsage(true, true)] public Color color;
        [ColorUsage(true, true)] public Color emission;
        [SerializeField] private Material material;

        public Material Mat {
            get {
                if (!material) {
                    material = new Material(MatchEnvironment.blockMaterial);
                    material.SetColor(Shader.PropertyToID("_BaseColor"), color);
                    material.SetColor(Shader.PropertyToID("_EmissionColor"), emission);
                }
                return material;
            }
        }
    }
}