using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;

namespace WinFormsUI.Game.Player.Stats
{
    public class GHeldEffect : GameComponent
    {
        public Effect Effect = null!;

        public GHeldEffect Init(Effect effect)
        {
            Effect = effect;
            return this;
        }
    }
}
