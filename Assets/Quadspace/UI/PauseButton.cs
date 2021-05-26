using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quadspace.Quadspace.UI {
    public class PauseButton : MonoBehaviour {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;

        private void Start() {
            button.onClick.AddListener(() => {
                if (Mathf.Approximately(Time.timeScale, 0)) {
                    label.text = "Pause";
                    Time.timeScale = 1;
                } else {
                    label.text = "Resume";
                    Time.timeScale = 0;
                }
            });
        }
    }
}