using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Quadspace.Quadspace.UI {
    public class CanvasScaleHelper : MonoBehaviour {
        private CanvasScaler scaler;

        private void Start() {
            scaler = GetComponent<CanvasScaler>();
            scaler.scaleFactor = PlayerPrefs.GetFloat("ui_scale", 1f);
        }

        private void Update() {
            if (Keyboard.current.pKey.wasPressedThisFrame) {
                scaler.scaleFactor += 0.1f;
                PlayerPrefs.SetFloat("ui_scale", scaler.scaleFactor);
            }
            
            if (Keyboard.current.oKey.wasPressedThisFrame && scaler.scaleFactor > 0.1f) {
                scaler.scaleFactor -= 0.1f;
                PlayerPrefs.SetFloat("ui_scale", scaler.scaleFactor);
            }
        }
    }
}