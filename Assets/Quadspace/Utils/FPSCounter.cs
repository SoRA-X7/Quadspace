using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Quadspace.Quadspace.Utils
{
    public class FPSCounter : MonoBehaviour {
        [SerializeField] private TMP_Text average;
        [SerializeField] private TMP_Text worst;
        [SerializeField] private TMP_Text fix;

        private readonly Queue<float> history = new Queue<float>();
        private int margin;

        private int fixedFrames;
        private int passedSeconds;

        private void Update() {
            if (margin++ < 10) return;
            
            var current = 1f / Time.unscaledDeltaTime;
            history.Enqueue(current);
            if (history.Count > 60) {
                history.Dequeue();
            }
            average.text = history.Average().ToString("F0", CultureInfo.CurrentCulture);
            worst.text = history.Min().ToString("F0", CultureInfo.CurrentCulture);
        }

        private void FixedUpdate() {
            if (Mathf.FloorToInt(Time.realtimeSinceStartup) > passedSeconds) {
                passedSeconds++;
                fix.text = fixedFrames.ToString();
                fixedFrames = 0;
            }
            fixedFrames++;
        }
    }
}
