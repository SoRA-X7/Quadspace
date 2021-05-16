using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Quadspace.Game.Interaction;
using Quadspace.Game.Moves;
using UnityEngine;
using Random = System.Random;

namespace Quadspace.Game {
    public class FieldBehaviour : MonoBehaviour {
        public Field field;
        public FallingPiece currentPiece;
        [SerializeField] private List<FallingPiece> nextPieces;
        [SerializeField] private FallingPiece holdPiece;
        private IGameController controller;

        [SerializeField] private Match match;
        [SerializeField] private FieldBlockManager blockManager;

        public CancellationTokenSource CancelTokenSource { get; private set; }

        private bool pieceAvailable;
        private string[] templateBag;
        private List<string> bag;
        private Random rand = new Random();

        public event Action ResetField;
        public event Action Initialized;
        public event Action<int> NewPiece;
        public event Action<Piece> PieceSpawned;

        public async UniTask Initialize() {
            CancelTokenSource?.Cancel();
            
            ResetField?.Invoke();

            await UniTask.DelayFrame(10);
            
            CancelTokenSource = new CancellationTokenSource();
            
            field = new Field(match.MatchEnv);
            currentPiece.Hide();
            holdPiece.Hide();
            templateBag = new[] {"I", "O", "T", "J", "L", "S", "Z"}; //TODO
            bag = new List<string>(templateBag);
            foreach (var t in nextPieces) {
                var p = GetRandomPieceFromBag();
                field.Next.Enqueue(p);
                t.SetOverwriteType(new Piece {kind = p});
            }

            blockManager.Clear();

            Initialized?.Invoke();

            UniTask.Run(async () => {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                SpawnPiece();
            });
        }

        public async UniTask LockPiece() {
            currentPiece.Hide();
            currentPiece.content = field.SonicDrop(currentPiece.content);
            var res = field.LockPiece(currentPiece.content);
            foreach (var pos in currentPiece.content.GetPositions()) {
                blockManager.AddBlock(pos, MatchEnvironment.pieceRegistry[currentPiece.content.kind].blockDescriptor);
            }

            for (var i = 0; i < res.clearedLines.Count; i++) {
                var y = res.clearedLines[i] - i;
                for (var x = 0; x < 10; x++) {
                    blockManager.RemoveBlock(new Vector2Int(x,y));
                }

                for (var y2 = y + 1; y2 < field.Rows.Count; y2++) {
                    for (var x = 0; x < 10; x++) {
                        blockManager.MoveBlock(new Vector2Int(x, y2), new Vector2Int(0, -1));
                    }
                }
            }

            await UniTask.DelayFrame(3, PlayerLoopTiming.FixedUpdate, CancelTokenSource.Token);

            SpawnPiece();
        }

        public void SpawnPiece(bool callEvent = true) {
            var spawnedKind = field.Next.Dequeue();
            currentPiece.SetOverwriteType(new Piece(spawnedKind, 4, 19, 0, SpinStatus.None));
            currentPiece.Show();
            if (callEvent) PieceSpawned?.Invoke(currentPiece.content);

            var add = GetRandomPieceFromBag();
            field.Next.Enqueue(add);
            NewPiece?.Invoke(add);

            var firstNext = nextPieces[0];
            nextPieces.RemoveAt(0);
            firstNext.transform.localPosition += new Vector3(0, -12, 0);
            firstNext.SetOverwriteType(new Piece {kind = add});
            foreach (var fp in nextPieces) {
                fp.transform.localPosition += new Vector3(0, 3, 0);
            }

            nextPieces.Add(firstNext);
        }

        private int GetRandomPieceFromBag() {
            var index = rand.Next(bag.Count);
            var piece = MatchEnvironment.pieceNameLookup[bag[index]].assignedID;
            bag.RemoveAt(index);
            if (bag.Count == 0) {
                bag.AddRange(templateBag);
            }

            return piece;
        }

        public void Hold() {
            if (field.Hold == null) {
                field.Hold = currentPiece.content.kind;
                holdPiece.SetOverwriteType(new Piece(currentPiece.content.kind, 0, 0, 0, SpinStatus.None));
                holdPiece.Show();
                currentPiece.SetOverwriteType(new Piece(field.Hold.Value, 4, 19, 0, SpinStatus.None));
                SpawnPiece(false);
            } else {
                var tmp = field.Hold.Value;
                field.Hold = currentPiece.content.kind;
                holdPiece.SetOverwriteType(new Piece(currentPiece.content.kind, 0, 0, 0, SpinStatus.None));
                currentPiece.SetOverwriteType(new Piece(tmp, 4, 19, 0, SpinStatus.None));
            }
        }
    }
}