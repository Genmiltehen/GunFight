using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WinFormsUI.Game.Scenes;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;
using static WinFormsUI.Game.Player.PlayerControlHelper;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class AimState : IPlayerState
    {
        private float _angle = 0;
        private const float Lim = 0.05f;
        private bool _isAiming;

        public string DebugName => "Aiming";

        public void Enter(GPlayer player, GScene scene)
        {
            if (player.HeadEntity.TryGet<GSprite>(out var hSprite)) hSprite
                    .SetTexture(scene.Assets.LoadTexture(player.HeadAiming), true);
            if (player.BodyEntity.TryGet<GSprite>(out var bSprite)) bSprite
                    .SetTexture(scene.Assets.LoadTexture(player.BodyAiming), true);
            _isAiming = true;
        }

        public void Exit(GPlayer player, GScene scene)
        {
            player.ShootTimer.Reset();
            PlayerHelper.SetFacing(player, new(player.IsRightFacing ? 1 : -1, 0));
        }

        private void Shoot(GPlayer player, GScene scene, float dt)
        {
            var ang = player.IsRightFacing ? _angle : MathF.PI - _angle;
            var tr = player.WeaponEntity.Transform;
            scene.Schedule(() =>
            {
                (scene as Level1Scene)?.projectileFactory.CreateProjectile("pistol_bullet", player.Name, scene, tr.RelativePosition2D, tr.Rotation);
            });
        }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (AimStart(player, scene.Input))
            {
                player.ShootTimer.Reset();
                _isAiming = true;
            }

            if (AimEnd(player, scene.Input))
            {
                Shoot(player, scene, dt);
                _isAiming = false;
            }

            if (!_isAiming && player.ShootTimer.Progress == 0) player.ShootTimer.Start();
            if (!_isAiming && player.ShootTimer.Progress == 1)
            {
                player.SwitchTo<IdleState>(scene);
                return;
            }

            var left = scene.Input.IsActionJustPressed($"left{player.Name}");
            var right = scene.Input.IsActionJustPressed($"right{player.Name}");

            if (!(left || right))
            {
                var ver = scene.Input.GetAxis($"Vertical{player.Name}");
                var dir = player.IsRightFacing ? 1 : -1;
                _angle += ver * dt;
                _angle = Math.Clamp(_angle, -MathF.PI / 2 + Lim, MathF.PI / 2 - Lim);

                var (sin, cos) = MathF.SinCos(_angle);
                PlayerHelper.SetFacing(player, new(dir * cos, sin));
                return;
            }

            if ((player.IsRightFacing && right) || (!player.IsRightFacing && left))
            {
                player.SwitchTo<WalkState>(scene);
                return;
            }

            PlayerHelper.SetFacing(player, new(right ? 1 : -1, 0));
            _angle = 0;
        }
    }
}
