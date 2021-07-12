using System;
using UnityEngine;

namespace Quadspace.Quadspace.Utils {
    public class ChangeVSync : MonoBehaviour {
        public void Toggle(int useVSync) {
            QualitySettings.vSyncCount = useVSync == 1 ? 1 : 0;
        }
    }
}