using WinFormsUI.Game.Combat.Weapons;
using XEngine.Core.Scenery;
using static WinFormsUI.Game.Player.Contol.ActionType;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class AimState : IPlayerState
    {
        private const float Lim = 0.05f;
        private const float LowerLimit = -MathF.PI / 2 + Lim;
        private const float UpperLimit = MathF.PI / 2 - Lim;
        private float _angle = 0;

        public string DebugName => "Aiming";

        public void Enter(GPlayer player, GScene scene)
        {
            if (player.Weaponry.HeldWeapon == null)
            {
                player.SwitchTo<IdleState>(scene);
                return;
            }

            player.Model.SetAiming();
            player.Model.SetWeaponTexture(player.Weaponry.HeldWeapon);
        }

        public void Exit(GPlayer player, GScene scene)
        {
            player.Model.SetFacingDiecration(new(player.IsRightFacing ? 1 : -1, 0));
            player.Model.SetWeaponTexture();
        }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (player.Control.Fetch("aim", ActionInactive))
            {
                player.SwitchTo<IdleState>(scene);
                return;
            }

            if (player.Control.Fetch("act", ActionActive)) player.Weaponry.Shoot();

            var left = player.Control.Fetch("left", ActionStart);
            var right = player.Control.Fetch("right", ActionStart);

            if (!(left || right))
            {
                _angle = Math.Clamp(_angle + player.Control.VerticalInput() * dt, LowerLimit, UpperLimit);

                var (sin, cos) = MathF.SinCos(_angle);
                player.Model.SetFacingDiecration(new((player.IsRightFacing ? 1 : -1) * cos, sin));
                return;
            }

            if ((player.IsRightFacing && right) || (!player.IsRightFacing && left))
            {
                player.SwitchTo<WalkState>(scene);
                return;
            }

            player.IsRightFacing = player.Model.SetFacingDiecration(new(right ? 1 : -1, 0));
            _angle = 0;
        }
    }
}
