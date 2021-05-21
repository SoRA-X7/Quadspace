using System;

namespace Quadspace.TBP {
    [Serializable]
    public struct TbpPieceLocation {
        public string type;
        public TbpPieceOrientation orientation;
        public int x;
        public int y;
    }
}