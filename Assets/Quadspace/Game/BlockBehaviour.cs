using Quadspace.Game.ScriptableObjects;
using UnityEngine;

namespace Quadspace.Game {
    public class BlockBehaviour : MonoBehaviour {
        [SerializeField] private new Renderer renderer;

        public void SetType(BlockDescriptor type) {
            var mat = type.material;
            renderer.enabled = mat;
            if (mat) {
                renderer.material = mat;
            }
        }
    }
}
