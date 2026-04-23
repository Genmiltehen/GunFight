using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Box2D
{
    [Flags]
    internal enum CollisionFlags : ulong
    {
        None = 0,
        GROUND = 1 << 0,
        PLAYER = 1 << 1,
        FOOT = 1 << 2,
    }
}
