using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Combat.Weapons;
using XEngine.Core.Base;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Combat
{
    public class WeaponItem
    {
        public readonly string TextureName = "";
        public readonly string ProjectileId = "";
        public readonly float Spread = 0;
        public readonly float MaxAmmo = 0;
        public readonly GameTimer FireTimer = new(float.MaxValue);

        public float CurrentAmmo = 0;

        public static WeaponItem None => new();

        public WeaponItem() { }
        public WeaponItem(WeaponConfig config)
        {
            TextureName = config.TexturePath;
            ProjectileId = config.ProjectileId;
            Spread = config.SpreadAngle;
            MaxAmmo = config.MaxAmmo;
            CurrentAmmo = MaxAmmo;
            FireTimer.Duration = 1.0f / config.FireRate;
        }
    }
}
