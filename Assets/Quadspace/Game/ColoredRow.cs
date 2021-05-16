using System;
using System.Collections.Generic;
using System.Linq;
using Quadspace.Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Quadspace.Game {
    public class ColoredRow {
        public readonly BlockDescriptor[] blocks;

        public ColoredRow(int xLength = 10) {
            blocks = new BlockDescriptor[xLength];
        }

        public BlockDescriptor this[int x] {
            get {
                CheckIndexer(x);
                return blocks[x];
            }
            set {
                CheckIndexer(x);
                blocks[x] = value;
            }
        }

        private void CheckIndexer(int x) {
            if (blocks == null) {
                throw new NullReferenceException("This row is not properly initialized");
            }
            if (x < 0 || x >= blocks.Length) {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
        }

        public bool Filled => blocks.All(b => b);
    }
}