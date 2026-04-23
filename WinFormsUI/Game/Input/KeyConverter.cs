using WinKeys = System.Windows.Forms.Keys;
using OtkKeys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace WinFormsUI.Game.Input
{
    /// <summary>
    /// Only supports A-Z and Arrow Keys
    /// </summary>
    public static class KeyConverter
    {
        private static readonly Dictionary<WinKeys, OtkKeys> Map = new() {
            { WinKeys.A, OtkKeys.A },
            { WinKeys.B, OtkKeys.B },
            { WinKeys.C, OtkKeys.C },
            { WinKeys.D, OtkKeys.D },
            { WinKeys.E, OtkKeys.E },
            { WinKeys.F, OtkKeys.F },
            { WinKeys.G, OtkKeys.G },
            { WinKeys.H, OtkKeys.H },
            { WinKeys.I, OtkKeys.I },
            { WinKeys.J, OtkKeys.J },
            { WinKeys.K, OtkKeys.K },
            { WinKeys.L, OtkKeys.L },
            { WinKeys.M, OtkKeys.M },
            { WinKeys.N, OtkKeys.N },
            { WinKeys.O, OtkKeys.O },
            { WinKeys.P, OtkKeys.P },
            { WinKeys.Q, OtkKeys.Q },
            { WinKeys.R, OtkKeys.R },
            { WinKeys.S, OtkKeys.S },
            { WinKeys.T, OtkKeys.T },
            { WinKeys.U, OtkKeys.U },
            { WinKeys.V, OtkKeys.V },
            { WinKeys.W, OtkKeys.W },
            { WinKeys.X, OtkKeys.X },
            { WinKeys.Y, OtkKeys.Y },
            { WinKeys.Z, OtkKeys.Z },

            { WinKeys.Up, OtkKeys.Up },
            { WinKeys.Down, OtkKeys.Down },
            { WinKeys.Left, OtkKeys.Left },
            { WinKeys.Right, OtkKeys.Right },

            { WinKeys.ShiftKey, OtkKeys.LeftShift },
            { WinKeys.Insert, OtkKeys.Insert },
            { WinKeys.Control, OtkKeys.LeftControl },
        };

        public static OtkKeys ToOpenTK(WinKeys winKey)
        {
            return Map.TryGetValue(winKey, out var otkKey) ? otkKey : OtkKeys.Unknown;
        }
    }
}
