using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utf8Json;

namespace Quadspace.Game {
    public class GameConfig {
        private const string DelaysFileName = "delays.json";
        private const string GraphicsFileName = "graphics.json";
        private const string LoggingFileName = "logging.json";
        public Dictionary<string, int> Delays { get; private set; }
        public Dictionary<string, int> Graphics { get; private set; }
        public Dictionary<string, int> Logging { get; private set; }

        public static GameConfig Load() {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Configs/"));
            
            var delays = CreateDefault(DelaysFileName, new Dictionary<string, int> {
                {"fixedFramerate", 60},
                {"placement", 3},
                {"clear1", 0},
                {"clear2", 0},
                {"clear3", 0},
                {"clear4", 0},
                {"usePcOverride", 1},
                {"pc", 0},
                {"hypertap", 1},
                {"softdrop", 2},
            });
            
            var graphics = CreateDefault(GraphicsFileName, new Dictionary<string, int> {
                {"useVSync", 1}
            });
            
            var logging = CreateDefault(LoggingFileName, new Dictionary<string, int> {
                {"logBotMessage", 0},
                {"logFrontendMessage", 0},
                {"logMove", 0}
            });

            return new GameConfig {
                Delays = delays,
                Graphics = graphics,
                Logging = logging
            };
        }

        private static Dictionary<string, int> CreateDefault(string fileName, Dictionary<string, int> content) {
            var path = Path.Combine(Environment.CurrentDirectory, "Configs/", fileName);
            if (!File.Exists(path)) {
                File.WriteAllBytes(path, JsonSerializer.Serialize(content));
                return content;
            } else {
                var des = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllBytes(path));

                var changed = false;
                foreach (var key in content.Keys) {
                    if (!des.ContainsKey(key)) {
                        des.Add(key, content[key]);
                        changed = true;
                    }
                }

                if (changed) {
                    File.WriteAllBytes(path, JsonSerializer.Serialize(des));
                }

                return des;
            }
        }

        public void Save() {
            Save(DelaysFileName, Delays);
            Save(GraphicsFileName, Graphics);
            Save(LoggingFileName, Logging);
        }

        private void Save(string fileName, Dictionary<string, int> content) {
            var path = Path.Combine(Environment.CurrentDirectory, "Configs/", fileName);
            File.WriteAllBytes(path, JsonSerializer.Serialize(content));
        }
    }
}