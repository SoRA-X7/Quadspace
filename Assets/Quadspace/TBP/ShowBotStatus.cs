using System;
using TMPro;
using UnityEngine;

namespace Quadspace.TBP {
    public class ShowBotStatus : MonoBehaviour {
        private TMP_Text text;
        private BotController botController;

        private void Start() {
            botController = GetComponentInParent<BotController>();
            text = GetComponent<TMP_Text>();
        }

        private void Update() {
            text.text = Stringify(botController.manager.Status);
        }

        private static string Stringify(BotStatus status) => status switch {
            BotStatus.NotInitialized => "Not Initialized",
            BotStatus.Initializing => "Initializing",
            BotStatus.Ready => "Ready",
            BotStatus.Running => "Running",
            BotStatus.Error => "Error",
            BotStatus.Quit => "Quit (no error)",
            _ => status.ToString()
        };
    }
}