using System.IO;
using UnityEngine;

namespace Quadspace.Quadspace.UI {
    public class OpenLogFile : MonoBehaviour {
        public void Open() {
            Application.OpenURL("file://" + Path.GetDirectoryName(Application.consoleLogPath));
        }
    }
}