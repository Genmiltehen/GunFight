using OpenTK.Mathematics;
using System.Diagnostics;
using WinFormsUI.Game.Combat.Projectiles;
using WinFormsUI.Game.Player;
using XEngine.Core.Base;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.Maths;

namespace WinFormsUI.Game.Combat.Weapons
{
    public class GPlayerWeaponry : GameComponent
    {
        public WeaponItem? _main = null;
        public WeaponItem? _aux = null;

        public WeaponItem? HeldWeapon => _main;

        public GPlayerWeaponry Init(string StartWeapon)
        {
            if (WeaponFactory.Instance.CreateWeapon(StartWeapon, out var w))
            {
                w.InitTexture(Owner.Scene.Assets);
                Owner.Scene.RegisterTimer(w.FireTimer);
                w.FireTimer.Start();
                Equip(w);
            }
            return this;
        }

        public void Equip(WeaponItem weapon) => _main ??= weapon;

        public void Swap() => (_main, _aux) = (_aux, _main);

        public void Shoot()
        {
            if (HeldWeapon == null || HeldWeapon.CurrentAmmo <= 0 || !HeldWeapon.FireTimer.IsFinished) return;
            HeldWeapon.CurrentAmmo--;
            HeldWeapon.FireTimer.Start();

            string projId = HeldWeapon.ProjectileId;
            var player = Owner.Get<GPlayer>()!;
            var scene = Owner.Scene;

            Vector2 MuzzlePoint = player.Owner.Transform.RelativePosition2D + player.Model.HeldWeaponMuzzleOffset;
            float dir = player.Model.WeaponEntity.Transform.Rotation;

            for (int i = 0; i < HeldWeapon.Shots; i++)
            {
                Vector2 spread = MathUtils.FromPolar(scene.GetRandomFloat() * HeldWeapon.Spread, scene.GetRandomFloat() * MathF.Tau);
                Vector2 vel = MathUtils.FromPolar(HeldWeapon.InitialVelocity, dir);
                scene.Schedule(() => ProjectileFactory.Instance.CreateProjectile(projId, player.Name, scene, MuzzlePoint, vel + spread));
            }
        }

        public void Drop()
        {
            if (HeldWeapon == null) return;

            // TODO: Spawn weapon item

            _main = null;
        }
    }
}
