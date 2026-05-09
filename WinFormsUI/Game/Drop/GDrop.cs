using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Combat.Weapons;
using XEngine.Core.Base;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Drop
{
    internal class GDrop : GameComponent, IDisposable
    {
        private WeaponItem? heldWeapon = null;
        public bool CanPickup { get; private set; }

        public GDrop Init(WeaponItem weapon)
        {
            heldWeapon = weapon;
            return this;
        }

        public GDrop SetPickupFlag(bool value)
        {
            CanPickup = value;
            return this;
        }

        public bool TryTake(out WeaponItem weapon)
        {
            Owner.MarkDelete();
            if (CanPickup && heldWeapon != null)
            {
                CanPickup = false;
                weapon = heldWeapon;
                heldWeapon = null;
                return true;
            }

            weapon = null!;
            return false;
        }

        public void Dispose()
        {
            if (heldWeapon != null) Owner.Scene.UnregisterTimer(heldWeapon.FireTimer);
        }
    }
}
