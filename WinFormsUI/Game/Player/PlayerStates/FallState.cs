using XEngine.Core.Scenery;
using static WinFormsUI.Game.Player.Contol.ActionType;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class FallState : IPlayerState
    {
        public string DebugName => "Falling";

        public void Enter(GPlayer player, GScene scene) { player.Model.SetFall(); }

        public void Exit(GPlayer player, GScene scene)
        {
            // TODO: animation end
        }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (player.Movement.IsOnGround)
            {
                if (player.JumpTimer.IsRunning) player.JumpTimer.Reset();
                player.SwitchTo<IdleState>(scene);
                return;
            }

            player.Movement.MovePlayer(player.Control.HorizotnalInput(), dt, 0.5f);

            var jump = player.Control.Fetch("jump", ActionActive);
            if (!jump) player.JumpTimer.Reset();

            if (jump && player.JumpTimer.IsRunning)
                player.Movement.ApplyImpulse(new(0, player.Stats.JumpPower * dt));

            if (player.Control.Fetch("jump", ActionEnd))
                player.Movement.ApplyImpulse(new(0, -player.Movement.GetVelocity().Y * 0.5f));
        }
    }
}
