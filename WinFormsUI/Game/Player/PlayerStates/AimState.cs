using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class AimState : IPlayerState
    {
        private float _angle = 0;
        private const float Lim = 0.05f;
        private bool _isAiming;

        public string DebugName => "Aiming";

        public void Enter(GPlayer player)
        {
            float x = player.WeaponEntity.Transform.Position2D.X;
            float y = player.WeaponEntity.Transform.Position2D.Y;
            player.WeaponEntity.Transform.Position2D = new(x, y + 0.3f);
            _isAiming = true;
        }

        public void Exit(GPlayer player)
        {
            float x = player.WeaponEntity.Transform.Position2D.X;
            float y = player.WeaponEntity.Transform.Position2D.Y;
            player.WeaponEntity.Transform.Position2D = new(x, y - 0.3f);
            player.ShootTimer.Reset();
            player.SetFacing(new(player.IsRightFacing ? 1 : -1, 0));
        }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (scene.Input.IsActionJustPressed($"shoot{player.Name}"))
            {
                player.ShootTimer.Reset();
                _isAiming = true;
            }

            if (scene.Input.IsActionJustReleased($"shoot{player.Name}"))
            {
                // shoot
                _isAiming = false;
            }

            Debug.WriteLine(player.ShootTimer.Progress);

            if (!_isAiming && player.ShootTimer.Progress == 0) player.ShootTimer.Start();
            if (!_isAiming && player.ShootTimer.Progress == 1)
            {
                player.SwitchTo<IdleState>();
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
                player.SetFacing(new(dir * cos, sin));
                return;
            }

            if ((player.IsRightFacing && right) || (!player.IsRightFacing && left))
            {
                player.SwitchTo<WalkState>();
                return;
            }

            player.SetFacing(new(right ? 1 : -1, 0));
            _angle = 0;
        }
    }
}
