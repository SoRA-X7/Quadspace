using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Quadspace.Game {
    public class Match : MonoBehaviour {
        public MatchEnvironment MatchEnv { get; private set; }
        public List<FieldBehaviour> fields = new List<FieldBehaviour>();
        
        public bool Ongoing { get; private set; }

        public void BeginMatch() {
            Ready();
            foreach (var fieldBehaviour in fields) {
                fieldBehaviour.Initialize().Forget();
            }

            Ongoing = true;
        }

        public void EndMatch() {
            foreach (var fieldBehaviour in fields) {
                fieldBehaviour.Stop();
            }

            Ongoing = false;
        }

        private void Ready() {
            MatchEnv = new MatchEnvironment();
            MatchEnv.SetRotationSystem(0);//todo
            MatchEnv.AddSpinDetector(0);
        }
    }
}