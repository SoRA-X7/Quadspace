using System;
using Quadspace.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Quadspace.Quadspace.UI {
    public class MatchControlPanel : MonoBehaviour {
        [SerializeField] private Button startButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button pauseButton2;
        [SerializeField] private Button stopButton;
        [SerializeField] private Image pauseBeforeStartImage;

        [SerializeField] private Match match;

        private void Start() {
            startButton.onClick.AddListener(ProcedureStart);
            restartButton.onClick.AddListener(ProcedureStart);
            pauseButton.onClick.AddListener(ProcedurePause);
            pauseButton2.onClick.AddListener(ProcedurePause);
            stopButton.onClick.AddListener(ProcedureStop);
        }

        private void Update() {
            startButton.gameObject.SetActive(!match.Ongoing);
            restartButton.gameObject.SetActive(match.Ongoing);
            pauseButton.gameObject.SetActive(!Mathf.Approximately(Time.timeScale, 0) || !match.Ongoing);
            pauseButton2.gameObject.SetActive(Mathf.Approximately(Time.timeScale, 0) && match.Ongoing);
            pauseBeforeStartImage.gameObject.SetActive(Mathf.Approximately(Time.timeScale, 0) && !match.Ongoing);
            stopButton.interactable = match.Ongoing;
        }

        private void ProcedureStart() {
            match.BeginMatch();
        }

        private void ProcedureStop() {
            Time.timeScale = 1f;
            match.EndMatch();
        }

        private void ProcedurePause() {
            Time.timeScale = Mathf.Approximately(Time.timeScale, 0) ? 1 : 0;
        }
    }
}