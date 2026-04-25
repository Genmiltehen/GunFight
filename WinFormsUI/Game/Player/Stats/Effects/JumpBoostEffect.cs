using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Player.Stats.Effects
{
    internal class JumpBoostEffect(float intensity) : PlayerEffect
    {
        public override float JumpPower => base.JumpPower * intensity;
    }
}
