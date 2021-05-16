using System.Collections.Generic;
using Quadspace.Game.ScriptableObjects;
using UnityEngine;

namespace Quadspace.Game {
    public class FieldBlockManager : MonoBehaviour {
        private readonly Stack<BlockBehaviour> pool = new Stack<BlockBehaviour>();
        [SerializeField] private BlockBehaviour prefab;

        private readonly Dictionary<Vector2Int, BlockBehaviour> instances = new Dictionary<Vector2Int, BlockBehaviour>();

        private BlockBehaviour Rent() {
            if (pool.Count == 0) {
                return Instantiate(prefab, transform);
            } else {
                var obj = pool.Pop();
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        private void Return(BlockBehaviour behaviour) {
            behaviour.gameObject.SetActive(false);
            pool.Push(behaviour);
        }

        public void AddBlock(Vector2Int pos, BlockDescriptor desc) {
            var obj = Rent();
            obj.SetType(desc);
            obj.transform.localPosition = (Vector2) pos;
            instances.Add(pos, obj);
        }

        public void RemoveBlock(Vector2Int pos) {
            if (!instances.ContainsKey(pos)) return;
            Return(instances[pos]);
            instances.Remove(pos);
        }

        public void MoveBlock(Vector2Int pos, Vector2Int offset) {
            if (!instances.ContainsKey(pos)) return;
            var obj = instances[pos];
            instances.Remove(pos);
            obj.transform.localPosition += (Vector3) (Vector2) offset;
            instances.Add(pos + offset, obj);
        }

        public void Clear() {
            foreach (var instance in instances.Values) {
                Return(instance);
            }
            
            instances.Clear();
        }
    }
}