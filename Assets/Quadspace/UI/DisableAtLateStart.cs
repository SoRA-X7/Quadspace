using System;
using UnityEngine;

namespace Quadspace.Quadspace.UI {
    [DefaultExecutionOrder(5000)]
    public class DisableAtLateStart : MonoBehaviour {
        private void Start() {
            gameObject.SetActive(false);
            Destroy(this);
        }
    }
}