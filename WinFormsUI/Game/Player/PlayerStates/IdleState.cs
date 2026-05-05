using XEngine.Core.Scenery;
using static WinFormsUI.Game.Player.Contol.ActionType;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class IdleState : IPlayerState
    {
        public string DebugName => "Idling";

        public void Enter(GPlayer player, GScene scene) { player.Model.SetIdling(); }

        public void Exit(GPlayer player, GScene scene) { }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (player.Control.Fetch("jump", ActionStart))
            {
                player.JumpTimer.Start();
                player.Movement.ApplyImpulse(new(0, player.Stats.JumpPower));
            }

            if (!player.Movement.IsOnGround) player.SwitchTo<FallState>(scene);
            else if (player.Control.HorizotnalInput() != 0) player.SwitchTo<WalkState>(scene);
            else if (player.Control.Fetch("aim", ActionActive)) player.SwitchTo<AimState>(scene);
        }
    }
}
