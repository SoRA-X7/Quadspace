using System;
using UnityEngine;

namespace Quadspace.Quadspace.Utils {
    public class ChangeFixedFramerate : MonoBehaviour {
        public void Change(int framerate) {
            if (framerate < 0 || framerate > 100000) {
                throw new ArgumentOutOfRangeException();
            }
            Time.fixedDeltaTime = 1f / framerate;
        }
    }
}