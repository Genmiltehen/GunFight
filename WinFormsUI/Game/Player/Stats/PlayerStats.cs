using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Player.Stats
{
    public class PlayerStats(float topSpeed, float acceleration, float jumpPower) : IPlayerStats
    {
        public float TopSpeed { get; private set; } = topSpeed;

        public float Acceleration { get; private set; } = acceleration;

        public float JumpPower { get; private set; } = jumpPower;
    }
}
