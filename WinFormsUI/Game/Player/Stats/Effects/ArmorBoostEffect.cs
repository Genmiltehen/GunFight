using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Player.Stats.Effects
{
    internal class ArmorBoostEffect(float intensity) : Effect
    {
        public override float Armor => base.Armor * intensity;
    }
}
