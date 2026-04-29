using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Config;

namespace WinFormsUI.Game.Combat.Projectiles
{
    public class ProjectileConfig : IIdentifilable
    {
        public string Id { get; set; } = "";

        public float Damage { get; set; }
        public float Size { get; set; }
        public float InitialVelocity { get; set; }

        public float MaxLifetime { get; set; }

        public string TexturePath { get; set; } = "";
        //public string TrailEffect { get; set; } = "";
        //public string ImpactSound { get; set; } = "";
    }
}
