using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Player.Stats
{
    public class PlayerStats(PlayerConfig config) : IPlayerStats
    {
        public float TopSpeed { get; private set; } = config.Speed;

        public float Acceleration { get; private set; } = config.Acceleration;

        public float JumpPower { get; private set; } = config.JumpPower;
    }
}
