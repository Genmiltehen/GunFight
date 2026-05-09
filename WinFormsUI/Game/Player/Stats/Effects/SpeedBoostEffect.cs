using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Player.Stats.Effects
{
    public class SpeedBoostEffect(float intensity) : Effect
    {
        public override float TopSpeed => base.TopSpeed * intensity;
        public override float Acceleration => base.Acceleration * intensity;
    }
}
