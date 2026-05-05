using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Player.Stats
{
    public interface IPlayerStats
    {
        public float TopSpeed { get; }
        public float Acceleration { get; }
        public float JumpPower { get; }
        public float Armor { get; }
    }
}
