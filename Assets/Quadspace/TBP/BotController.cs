using Quadspace.Game;
using SFB;
using TMPro;
using UnityEngine;

namespace Quadspace.TBP {
    public class BotController : MonoBehaviour {
        private const string PrefKeyBotExe = "bot_exe";
        private FieldBehaviour fb;
        private GameInputProcessor controller;
        public BotManager manager;

        private string botExePath;
        [SerializeField] private TMP_Text botExePathText;
        [SerializeField] private int botIndex;

        private void Start() {
            fb = GetComponent<FieldBehaviour>();
            controller = new GameInputProcessor(fb);

            var savedExePath = PlayerPrefs.GetString(PrefKeyBotExe, "");
            if (savedExePath != "") {
                botExePath = savedExePath;
                manager = new BotManager(botExePath, controller, botIndex);
                botExePathText.text = System.IO.Path.GetFileName(botExePath);
            }

            fb.ResetField += () => {
                manager?.Dispose();
            };

            fb.Initialized += () => {
                if (!string.IsNullOrEmpty(botExePath)) {
                    manager = new BotManager(botExePath, controller, botIndex);
                    manager.Launch(fb);
                }
            };
        }

        public void SelectBotExe() {
            botExePath = null;
            botExePathText.text = "Not Set";
            PlayerPrefs.SetString(PrefKeyBotExe, "");
            manager?.Dispose();
            manager = null;
            var paths = StandaloneFileBrowser.OpenFilePanel("Choose your bot executable", "", "", false);
            if (paths.Length != 1) return;
            botExePath = paths[0];
            botExePathText.text = System.IO.Path.GetFileName(botExePath);
            PlayerPrefs.SetString(PrefKeyBotExe, botExePath);
        }

        private void OnDestroy() {
            manager?.Dispose();
        }
    }
}