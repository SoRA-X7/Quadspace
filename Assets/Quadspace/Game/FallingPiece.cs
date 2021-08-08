using System.Collections.Generic;
using System.Linq;
using Quadspace.Game.ScriptableObjects;
using UnityEngine;

namespace Quadspace.Game {
    public class FallingPiece : MonoBehaviour {
        public Piece content;

        private List<BlockBehaviour> blocks = new List<BlockBehaviour>();

        [SerializeField] private BlockBehaviour blockPrefab;

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public void SetOverwriteType(Piece piece) {
            content = piece;
            var descriptor = MatchEnvironment.GetBlockFromPiece(piece.kind);
            var positions = piece.GetPositions().ToList(); //TODO reduce allocation
            while (blocks.Count < positions.Count) {
                blocks.Add(Instantiate(blockPrefab, transform));
            }
            
            foreach (var block in blocks) {
                block.SetType(descriptor);
            }

            for (var i = 0; i < blocks.Count; i++) {
                if (i < positions.Count) {
                    blocks[i].gameObject.SetActive(true);
                    blocks[i].transform.localPosition = (Vector2) positions[i];
                } else {
                    blocks[i].gameObject.SetActive(false);
                }
            }
        }

        public void Set(Piece newPiece) {
            content = newPiece;
            var positions = newPiece.GetPositions().ToList(); //TODO reduce allocation
            for (var i = 0; i < positions.Count; i++) {
                if (blocks[i]) {
                    blocks[i].transform.localPosition = (Vector2) positions[i];
                }
            }
        }
    }
}