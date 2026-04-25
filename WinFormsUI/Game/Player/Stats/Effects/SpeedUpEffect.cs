using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Player.Stats.Effects
{
    public class SpeedUpEffect(float _intensity) : PlayerEffect
    {
        public override float TopSpeed => base.TopSpeed * _intensity;
        public override float Acceleration => base.Acceleration * _intensity;
    }
}
