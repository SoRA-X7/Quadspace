using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quadspace.Game.ScriptableObjects {
    [CreateAssetMenu]
    public class PieceDescriptor : IndexedScriptableObject {
        public new string name;
        public string block;
        public BlockDescriptor blockDescriptor;
        [SerializeField] private List<Vector2Int> blocks;

        public List<List<Vector2Int>> Occupations { get; private set; }
        public List<List<Vector2Int?>> Canonicals { get; private set; }

        private void OnEnable() {
            Occupations = new List<List<Vector2Int>> {
                blocks.ToList(),
                blocks.Select(v => new Vector2Int(v.y, -v.x)).ToList(),
                blocks.Select(v => new Vector2Int(-v.x, -v.y)).ToList(),
                blocks.Select(v => new Vector2Int(-v.y, v.x)).ToList()
            };

            Canonicals = new List<List<Vector2Int?>>(4);
            for (var s = 0; s < 4; s++) {
                Canonicals.Add(new List<Vector2Int?>());
                Canonicals[s].AddRange(Enumerable.Repeat((Vector2Int?)null, 4));
                for (var x = -5; x < 5; x++) {
                    for (var y = -5; y < 5; y++) {
                        var offset = new Vector2Int(x, y);
                        for (var s2 = 0; s2 < 4; s2++) {
                            if (s2 == s) continue;

                            if (Occupations[s].All(b => Occupations[s2].Contains(b - offset))) {
                                Canonicals[s][s2] = offset;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}