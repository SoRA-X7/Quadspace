using System;
using Quadspace.Game;

namespace Quadspace.TBP {
    [Serializable]
    public struct TbpMove {
        public TbpPieceLocation location;
        public TbpPieceSpinStatus spin;

        public static implicit operator Piece(TbpMove move) {
            return new Piece(
                MatchEnvironment.pieceNameLookup[move.location.type].assignedID,
                move.location.x,
                move.location.y,
                (int) move.location.orientation,
                (SpinStatus) move.spin);
        }

        public static implicit operator TbpMove(Piece piece) {
            return new TbpMove {
                location = new TbpPieceLocation {
                    type = MatchEnvironment.pieceRegistry[piece.kind].name,
                    x = piece.x,
                    y = piece.y,
                    orientation = (TbpPieceOrientation) piece.r
                },
                spin = (TbpPieceSpinStatus) piece.spin
            };
        }
    }
}