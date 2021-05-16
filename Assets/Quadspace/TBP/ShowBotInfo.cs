using System;
using Quadspace.Quadspace.TBP.Messages;
using TMPro;
using UnityEngine;

namespace Quadspace.Quadspace.TBP {
    public class ShowBotInfo : MonoBehaviour {
        [SerializeField] private TMP_Text name;
        [SerializeField] private TMP_Text author;
        [SerializeField] private TMP_Text version;
        [SerializeField] private TMP_Text features;

        private TbpInfoMessage cachedInfo;
        private BotController botController;

        private void Start() {
            botController = GetComponentInParent<BotController>();
        }

        private void Update() {
            if (botController.manager?.BotInfo == cachedInfo) return;
            cachedInfo = botController.manager?.BotInfo;
            
            name.text = cachedInfo?.name ?? "";
            author.text = cachedInfo?.author ?? "";
            version.text = cachedInfo?.version ?? "";
            features.text = cachedInfo != null ? string.Concat(cachedInfo.features) : "";
        }
    }
}