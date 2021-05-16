using System.Collections.Generic;
using UnityEngine;

namespace Quadspace.Game {
    public class Match : MonoBehaviour {
        public MatchEnvironment MatchEnv { get; private set; }
        public List<FieldBehaviour> fields = new List<FieldBehaviour>();

        private void Awake() {
            MatchEnv = new MatchEnvironment();
            MatchEnv.SetRotationSystem(0);//todo
        }

        public void BeginMatch() {
            foreach (var fieldBehaviour in fields) {
                fieldBehaviour.Initialize();
            }
        }
    }
}