using System;
using Quadspace.Game;

namespace Quadspace.Quadspace.TBP {
    [Serializable]
    public struct TbpPieceLocation {
        public string type;
        public TbpPieceOrientation orientation;
        public int x;
        public int y;
    }
}