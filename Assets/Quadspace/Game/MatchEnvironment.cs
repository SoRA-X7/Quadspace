using System;
using System.Collections.Generic;
using System.IO;
using Quadspace.Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quadspace.Game {
    public class MatchEnvironment {
        public static readonly List<BlockDescriptor> blockRegistry = new List<BlockDescriptor>();
        public static readonly List<PieceDescriptor> pieceRegistry = new List<PieceDescriptor>();
        public static readonly List<RotationSystemDescriptor> rotationSystemRegistry =
            new List<RotationSystemDescriptor>();
        public static readonly List<SpinDetectorDescriptor> SpinDetectorRegistry =
            new List<SpinDetectorDescriptor>();
        public static readonly Dictionary<string, PieceDescriptor> pieceNameLookup =
            new Dictionary<string, PieceDescriptor>();
        public static readonly Dictionary<string, BlockDescriptor> blockNameLookup =
            new Dictionary<string, BlockDescriptor>();

        public static GameConfig config = GameConfig.Load();

        public static Material blockMaterial;

        public RotationSystemDescriptor rotationSystemInUse;
        public Dictionary<int, RotationSystemModuleDescriptor> rotationSystemLookup =
            new Dictionary<int, RotationSystemModuleDescriptor>();

        public Dictionary<int, SpinDetectorDescriptor> spinDetectorLookup =
            new Dictionary<int, SpinDetectorDescriptor>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Load() {
            Debug.Log("Load");
            CreateDirectories();
            ExportPresets();
            
            blockMaterial = Addressables.LoadAssetAsync<Material>("Assets/Materials/Block.mat").WaitForCompletion();
            
            Addressables.LoadAssetsAsync<BlockDescriptor>("blocks", d => {
                Register(d, "Block", blockRegistry);
                blockNameLookup.Add(d.name, d);
            }).WaitForCompletion();
            Addressables.LoadAssetsAsync<PieceDescriptor>("pieces", d => {
                Register(d, "Piece", pieceRegistry);
                pieceNameLookup.Add(d.name, d);
            }).WaitForCompletion();
            Addressables.LoadAssetsAsync<RotationSystemDescriptor>("rotation_systems",
                d => {
                    Register(d, "Rotation System", rotationSystemRegistry);
                }).WaitForCompletion();
            Addressables.LoadAssetsAsync<SpinDetectorDescriptor>("spin_detectors", d => {
                Register(d, "Spin Detector", SpinDetectorRegistry);
            });
        }

        private static void CreateDirectories() {
            var wd = Path.Combine(Environment.CurrentDirectory, "Definitions/");
            Directory.CreateDirectory(Path.Combine(wd, "Blocks/"));
            Directory.CreateDirectory(Path.Combine(wd, "Pieces/"));
            Directory.CreateDirectory(Path.Combine(wd, "RotationSystems/"));
            Directory.CreateDirectory(Path.Combine(wd, "SpinDetectors/"));
            Debug.Log(wd);
        }

        private static void ExportPresets() {
            Addressables.LoadAssetsAsync<BlockDescriptor>("blocks", d => {
                using var writer = File.CreateText(
                    Path.Combine(Environment.CurrentDirectory, "Definitions/Blocks/", d.name) + ".json");
                writer.Write(JsonUtility.ToJson(d));
            });
            Addressables.LoadAssetsAsync<PieceDescriptor>("pieces", d => {
                using var writer = File.CreateText(
                    Path.Combine(Environment.CurrentDirectory, "Definitions/Pieces/", d.name) + ".json");
                writer.Write(JsonUtility.ToJson(d));
            });
            Addressables.LoadAssetsAsync<RotationSystemDescriptor>("rotation_systems", d => {
                var dir = Path.Combine(Environment.CurrentDirectory, "Definitions/RotationSystems/", d.name);
                Directory.CreateDirectory(dir);
                foreach (var module in d.modules) {
                    using var writer = File.CreateText(dir + "/" + module.name + ".json");
                    writer.Write(JsonUtility.ToJson(module));
                }
            });
            Addressables.LoadAssetsAsync<SpinDetectorDescriptor>("spin_detectors", d => {
                using var writer = File.CreateText(
                    Path.Combine(Environment.CurrentDirectory, "Definitions/SpinDetectors/", d.name) + ".json");
                writer.Write(JsonUtility.ToJson(d));
            });
        }

        private static void Register<T>(T item, string type, List<T> registry) where T : IndexedScriptableObject {
            item.assignedID = registry.Count;
            registry.Add(item);
            Debug.Log($"{type} {item.name} LOADED as #{item.assignedID.ToString()}");
        }

        public static BlockDescriptor GetBlockFromPiece(int pieceID) {
            return blockNameLookup[pieceRegistry[pieceID].block];
        }

        public void SetRotationSystem(int index) {
            rotationSystemInUse = rotationSystemRegistry[index];
            foreach (var module in rotationSystemInUse.modules) {
                foreach (var pieceStr in module.apply) {
                    rotationSystemLookup.Add(pieceNameLookup[pieceStr].assignedID, module);
                }
            }
        }

        public void AddSpinDetector(int index) {
            var d = SpinDetectorRegistry[index];
            spinDetectorLookup.Add(pieceNameLookup[d.applyPiece].assignedID, d);
        }
    }
}