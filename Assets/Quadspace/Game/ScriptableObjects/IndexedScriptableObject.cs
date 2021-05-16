using System;
using UnityEngine;

namespace Quadspace.Game.ScriptableObjects {
    public class IndexedScriptableObject : ScriptableObject {
        [NonSerialized] public int assignedID;
    }
}