using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class IdleState : IPlayerState
    {
        public string DebugName => "Idling";

        public void Enter(GPlayer player) { }

        public void Exit(GPlayer player) { }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            var b2body = player.Owner.Get<GBox2DBody>()!;
            if (scene.Input.IsActionJustPressed($"jump{player.Name}"))
            {
                player.JumpTimer.Start();
                B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(0, player.Stats.JumpPower), true);
            }

            if (!player.IsOnGround())
            {
                player.SwitchTo<FallState>();
                return;
            }

            if (player.HorizontalInput(scene.Input) != 0)
            {
                player.SwitchTo<WalkState>();
                return;
            }

            var shoot = scene.Input.IsActionActive($"shoot{player.Name}");
            if (shoot)
            {
                player.SwitchTo<AimState>();
                return;
            }
        }
    }
}
