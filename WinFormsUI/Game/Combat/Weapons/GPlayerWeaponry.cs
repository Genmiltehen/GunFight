using OpenTK.Mathematics;
using System.Diagnostics;
using WinFormsUI.Game.Combat.Projectiles;
using WinFormsUI.Game.Drop;
using WinFormsUI.Game.Player;
using XEngine.Core.Base;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.Maths;

namespace WinFormsUI.Game.Combat.Weapons
{
    public class GPlayerWeaponry : GameComponent
    {
        public WeaponItem? HeldWeapon { get; private set; } = null;
        public WeaponItem? AuxWeapon { get; private set; } = null;

        public GPlayerWeaponry Init(string StartWeapon)
        {
            if (WeaponFactory.Instance.TryCreateWeapon(StartWeapon, out var w))
            {
                w.Init(Owner.Scene);
                Equip(w);
            }
            return this;
        }

        public bool TryTake(out WeaponItem weapon)
        {
            bool res = HeldWeapon != null;
            weapon = HeldWeapon!;
            HeldWeapon = null;
            return res;
        }

        public bool Equip(WeaponItem? weapon)
        {
            if (HeldWeapon == null)
            {
                HeldWeapon = weapon;
                return true;
            }
            return false;
        }

        public void Swap() => (HeldWeapon, AuxWeapon) = (AuxWeapon, HeldWeapon);
    }
}
