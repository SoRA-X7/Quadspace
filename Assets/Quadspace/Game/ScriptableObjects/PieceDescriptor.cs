using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quadspace.Game.ScriptableObjects {
    [CreateAssetMenu]
    public class PieceDescriptor : IndexedScriptableObject {
        public new string name;
        public BlockDescriptor blockDescriptor;
        [SerializeField] private List<Vector2Int> blocks;

        private List<List<Vector2Int>> occupations;

        public List<Vector2Int> GetBlocks(int spin) {
            occupations ??= new List<List<Vector2Int>> {
                blocks.ToList(),
                blocks.Select(v => new Vector2Int(v.y, -v.x)).ToList(),
                blocks.Select(v => new Vector2Int(-v.x, -v.y)).ToList(),
                blocks.Select(v => new Vector2Int(-v.y, v.x)).ToList()
            };
            return occupations[spin];
        }
    }
}