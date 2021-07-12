using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quadspace.Game {
    public class Field {
        public List<ColoredRow> Rows { get; } = new List<ColoredRow>();
        public Queue<int> Next { get; } = new Queue<int>();
        public int? Hold { get; set; } = null;
        public int Ren { get; set; } = -1;
        public bool BackToBack { get; set; } = false;

        private MatchEnvironment environment;
        private readonly Vector2Int fieldSize;


        public Field(MatchEnvironment env, Vector2Int fieldSize) {
            environment = env;
            this.fieldSize = fieldSize;
            for (var y = 0; y < fieldSize.y; y++) {
                Rows.Add(new ColoredRow(fieldSize.x));
            }
        }

        public Field(MatchEnvironment env) : this(env, new Vector2Int(10, 44)) { }


        public bool Collides(in Piece piece) {
            foreach (var position in piece.GetPositions()) {
                if (position.x < 0 || position.x >= fieldSize.x) return true;
                if (position.y < 0 || position.y >= fieldSize.y) return true;
                if (Rows[position.y][position.x]) return true;
            }

            return false;
        }

        public Piece SonicDrop(Piece piece) {
            while (true) {
                var prev = piece;
                piece.y -= 1;
                if (Collides(piece)) {
                    return prev;
                }
            }
        }

        public Piece? Rotate(Piece piece, bool cw) {
            var rs = piece.GetRotationSystem(environment);
            var sd = piece.GetSpinDetector(environment);

            if (!rs) return null;

            var to = (piece.r + (cw ? 1 : 3)) % 4;
            var i = 0;
            foreach (var offset in rs.Get(piece.r, cw)) {
                var rot = new Piece(piece.kind, piece.x + offset.x, piece.y + offset.y, to, SpinStatus.None);
                if (sd) {
                    rot = new Piece(piece.kind, piece.x + offset.x, piece.y + offset.y, to, sd.CheckSpin(this, rot, i));
                }
                if (!Collides(rot)) {
                    return rot;
                }

                i++;
            }

            return null;
        }

        public bool Grounded(Piece piece) {
            return Collides(piece.Strafe(0, -1));
        }

        public LockResult LockPiece(Piece piece) {
            foreach (var pos in piece.GetPositions()) {
                Rows[pos.y][pos.x] = MatchEnvironment.pieceRegistry[piece.kind].blockDescriptor;
            }

            var clearedLines = new List<int>();
            for (var i = 0; i < Rows.Count; i++) {
                if (Rows[i].Filled) {
                    Rows.RemoveAt(i);
                    Rows.Add(new ColoredRow(fieldSize.x));
                    clearedLines.Add(i + clearedLines.Count);
                    i -= 1;
                }
            }

            var pc = true;
            foreach (var row in Rows) {
                if (!row.Empty) {
                    pc = false;
                    break;
                }
            }

            return new LockResult {
                clearedLines = clearedLines,
                kind = PlacementKindFactory.Create(clearedLines.Count, piece.spin),
                pc = pc
            };
        }

        public bool Occupied(Vector2Int pos) {
            if (pos.x < 0 || pos.y < 0 || pos.x >= fieldSize.x || pos.y >= fieldSize.y) return true;
            return Rows[pos.y][pos.x];
        }

        public Field Clone() {
            var cl = new Field(environment, fieldSize) {
                BackToBack = BackToBack,
                Hold = Hold,
                Ren = Ren
            };
            for (var i = 0; i < Rows.Count; i++) {
                cl.Rows[i] = Rows[i].Clone();
            }

            return cl;
        }
    }
}