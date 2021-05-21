using TMPro;
using UnityEngine;

namespace Quadspace.TBP {
    public class ShowMoveInfo : MonoBehaviour {
        [SerializeField] private TMP_Text moveIndex;
        [SerializeField] private TMP_Text suggestionRespondTime;
        
        private BotController botController;

        private void Start() {
            botController = GetComponentInParent<BotController>();
        }

        private void Update() {
            moveIndex.text = botController.manager?.PickedMoveIndex.ToString() ?? "N/A";
            suggestionRespondTime.text = (botController.manager?.ResponseTimeInMillisecond.ToString() ?? "N/A ") + "ms";
        }
    }
}