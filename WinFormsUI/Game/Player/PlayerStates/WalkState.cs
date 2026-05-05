using XEngine.Core.Scenery;
using static WinFormsUI.Game.Player.Contol.ActionType;

namespace WinFormsUI.Game.Player.PlayerStates
{
    public class WalkState : IPlayerState
    {
        public string DebugName => "Walking";
        private int Frame = 0;

        public void Enter(GPlayer player, GScene scene)
        {
            player.Model.SetMoving(Frame);
            player.WalkTimer.Start();
        }

        public void Exit(GPlayer player, GScene scene) { }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (player.Control.Fetch("jump", ActionStart))
            {
                player.JumpTimer.Start();
                player.Movement.ApplyImpulse(new(0, player.Stats.JumpPower));
            }

            if (!player.WalkTimer.IsRunning)
            {
                Frame = (Frame + 1) % 4;
                player.Model.SetMoving(Frame);
                player.WalkTimer.Start();
            }

            var hor = player.Control.HorizotnalInput();
            if (hor == 0)
            {
                player.SwitchTo<IdleState>(scene);
                return;
            }
            player.Movement.MovePlayer(hor, dt);

            if (!player.Movement.IsOnGround) player.SwitchTo<FallState>(scene);
            else if (player.Control.Fetch("aim", ActionStart)) player.SwitchTo<AimState>(scene);
        }
    }
}
