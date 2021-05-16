using System.Globalization;
using TMPro;
using UnityEngine;

namespace Quadspace.Quadspace.Utils
{
    public class FPSCounter : MonoBehaviour {
        [SerializeField] private TMP_Text text;

        private void Update() {
            text.text = (1f / Time.deltaTime).ToString(CultureInfo.CurrentCulture);
        }
    }
}
