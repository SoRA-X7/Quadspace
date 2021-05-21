using System;
using TMPro;
using UnityEngine;

namespace Quadspace.Quadspace.TBP {
    public class ShowMoveInfo : MonoBehaviour {
        [SerializeField] private TMP_Text moveIndex;
        
        private BotController botController;

        private void Start() {
            botController = GetComponentInParent<BotController>();
        }

        private void Update() {
            moveIndex.text = botController.manager?.PickedMoveIndex.ToString() ?? "";
        }
    }
}