using static Quadspace.Game.PlacementKind;

namespace Quadspace.Game {
    public enum PlacementKind : byte {
        None = 0b00_000,
        Clear1 = 0b00_001,
        Clear2 = 0b00_010,
        Clear3 = 0b00_011,
        Clear4 = 0b00_100,
        Mini = 0b01_000,
        Mini1 = 0b01_001,
        Mini2 = 0b01_010,
        Spin = 0b10_000,
        Spin1 = 0b10_001,
        Spin2 = 0b10_010,
        Spin3 = 0b10_011
    }

    public static class PlacementKindExtensions {
        public static uint GetGarbage(this PlacementKind placementKind) {
            return placementKind switch {
                Clear2 => 1,
                Mini2 => 1,
                Clear3 => 2,
                Spin1 => 2,
                Clear4 => 4,
                Spin2 => 4,
                Spin3 => 6,
                _ => 0
            };
        }

        public static bool IsContinuous(this PlacementKind placementKind) {
            return placementKind >= Clear4;
        }

        public static bool IsLineClear(this PlacementKind placementKind) {
            return ((byte) placementKind & 0b111) != 0;
        }

        public static bool IsFullTspinClear(this PlacementKind placementKind) {
            return ((byte) placementKind & 0b10_000) != 0 && placementKind.IsLineClear();
        }
    }

    public static class PlacementKindFactory {
        public static PlacementKind Create(int clearedLine, SpinStatus spin) {
            return (PlacementKind) ((int) spin * 8 + clearedLine);
        }
    }

}