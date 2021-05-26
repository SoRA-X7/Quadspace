using System;
using Quadspace.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quadspace.Quadspace.UI {
    public class DelaysSelector : MonoBehaviour {
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Toggle toggle;
        [SerializeField] private string key;

        private void Start() {
            if (input) {
                input.text = MatchEnvironment.config.Delays[key].ToString();
                input.onValueChanged.AddListener(str => Overwrite(int.Parse(str)));
            } else if (toggle) {
                toggle.isOn = MatchEnvironment.config.Delays[key] == 1;
                toggle.onValueChanged.AddListener(b => Overwrite(b ? 1 : 0));
            }
        }

        private void Overwrite(int val) {
            MatchEnvironment.config.Delays[key] = val;
            MatchEnvironment.config.Save();
        }
    }
}