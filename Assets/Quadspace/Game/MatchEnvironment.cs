using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Quadspace.Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quadspace.Game {
    public class MatchEnvironment {
        public static readonly List<BlockDescriptor> blockRegistry = new List<BlockDescriptor>();
        public static readonly List<PieceDescriptor> pieceRegistry = new List<PieceDescriptor>();
        public static readonly Dictionary<string, PieceDescriptor> pieceNameLookup = new Dictionary<string, PieceDescriptor>();
        public static readonly List<RotationSystemDescriptor> rotationSystemRegistry = new List<RotationSystemDescriptor>();

        public RotationSystemDescriptor rotationSystemInUse;

        public Dictionary<PieceDescriptor, RotationSystemModuleDescriptor> rotationSystemLookup =
            new Dictionary<PieceDescriptor, RotationSystemModuleDescriptor>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Load() {
            Addressables.LoadAssetsAsync<BlockDescriptor>("blocks", d => Register(d, "Block", blockRegistry)).WaitForCompletion();
            Addressables.LoadAssetsAsync<PieceDescriptor>("pieces", d => {
                Register(d, "Piece", pieceRegistry);
                pieceNameLookup.Add(d.name, d);
            }).WaitForCompletion();
            Addressables.LoadAssetsAsync<RotationSystemDescriptor>("rotation_systems",
                d => Register(d, "Rotation System", rotationSystemRegistry)).WaitForCompletion();
        }

        private static void Register<T>(T item, string type, List<T> registry) where T : IndexedScriptableObject {
            item.assignedID = registry.Count;
            registry.Add(item);
            Debug.Log($"{type} {item.name} LOADED as #{item.assignedID.ToString()}");
        }

        public void SetRotationSystem(int index) {
            rotationSystemInUse = rotationSystemRegistry[index];
            foreach (var module in rotationSystemInUse.modules) {
                foreach (var pieceStr in module.apply) {
                    rotationSystemLookup.Add(pieceNameLookup[pieceStr], module);
                }
            }
        }
    }
}