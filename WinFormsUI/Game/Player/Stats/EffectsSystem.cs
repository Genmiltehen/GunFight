using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Player.Stats
{
    public class EffectsSystem : IGameSystem
    {
        public int Priority => 200;

        public bool IsEnabled { get; set; } = true;

        public void Update(GScene _scene, float _dt)
        {
            foreach (var (_, eff) in _scene.Query<GEffects>()) eff.WearoffEffects();
        }
    }
}
