using XEngine.Core.Base;

namespace WinFormsUI.Game.Combat.Weapons
{
    public class GWeaponry : GameComponent
    {
        public WeaponItem? HeldWeapon { get; private set; } = null;
        public WeaponItem? AuxWeapon { get; private set; } = null;

        public GWeaponry Init(string StartWeapon)
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
