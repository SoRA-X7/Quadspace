using System;
using System.Collections.Generic;
using Quadspace.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Quadspace.Quadspace.UI {
    public class ConfigSelector : MonoBehaviour {
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Toggle toggle;
        [SerializeField] private ConfigCategory category;
        [SerializeField] private string key;

        public UnityEvent<int> onValueChanged;

        private void Start() {
            onValueChanged.Invoke(Registry()[key]);
            if (input) {
                input.text = Registry()[key].ToString();
                input.onEndEdit.AddListener(str => Overwrite(int.Parse(str)));
            } else if (toggle) {
                toggle.isOn = Registry()[key] == 1;
                toggle.onValueChanged.AddListener(b => Overwrite(b ? 1 : 0));
            }
        }

        private void Overwrite(int val) {
            Registry()[key] = val;
            onValueChanged.Invoke(val);
            MatchEnvironment.config.Save();
        }

        private Dictionary<string, int> Registry() {
            return category switch {
                ConfigCategory.Delays => MatchEnvironment.config.Delays,
                ConfigCategory.Graphics => MatchEnvironment.config.Graphics,
                ConfigCategory.Logging => MatchEnvironment.config.Logging,
            };
        }
    }
}