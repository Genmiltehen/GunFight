using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Config;

namespace WinFormsUI.Game.Combat.Weapons
{
    public class WeaponConfig : IIdentifilable
    {
        public string Id { get; set; } = "";

        public int MaxAmmo { get; set; }
        public float FireRate { get; set; }
        public float SpreadAngle { get; set; }

        public string ProjectileId { get; set; } = "";

        public string TexturePath { get; set; } = "";
        //public string FireSoundPath { get; set; } = "";
    }
}
