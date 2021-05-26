using System;
using System.Collections.Generic;
using System.IO;
using Utf8Json;

namespace Quadspace.Game {
    public class GameConfig {
        public Dictionary<string, int> Delays { get; private set; }

        public static GameConfig Load() {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Configs/"));
            
            var path = Path.Combine(Environment.CurrentDirectory, "Configs/", "delays.json");
            if (!File.Exists(path)) {
                File.WriteAllBytes(path, JsonSerializer.Serialize(new Dictionary<string, int>() {
                    {"placement", 3},
                    {"clear1", 0},
                    {"clear2", 0},
                    {"clear3", 0},
                    {"clear4", 0},
                    {"usePcOverride", 1},
                    {"pc", 0},
                    {"hypertap", 1},
                    {"softdrop", 2},
                }));
            }

            return new GameConfig {
                Delays = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllBytes(path))
            };
        }

        public void Save() {
            var path = Path.Combine(Environment.CurrentDirectory, "Configs/", "delays.json");
            File.WriteAllBytes(path, JsonSerializer.Serialize(Delays));
        }
    }
}