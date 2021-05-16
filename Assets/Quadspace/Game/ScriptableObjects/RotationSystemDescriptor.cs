using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quadspace.Game.ScriptableObjects {
    [CreateAssetMenu]
    public class RotationSystemDescriptor : IndexedScriptableObject {
        public string name;
        public List<RotationSystemModuleDescriptor> modules;
    }
}