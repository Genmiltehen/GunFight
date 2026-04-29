using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Combat.Projectiles
{
    public class GProjectile : GameComponent
    {
        public GameTimer LifeTimer = new(0);
        public string Source { get; set; } = null!;
        public float Damage { get; set; } = 0;

        public GProjectile Init(ProjectileConfig config, GScene scene)
        {
            scene.RegisterTimer(LifeTimer);
            LifeTimer.Duration = config.MaxLifetime;
            Damage = config.Damage;
            return this;
        }

        public GProjectile SetSource(string sourceName)
        {
            Source = sourceName;
            return this;
        }
    }
}
