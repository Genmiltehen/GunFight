using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.Maths;

namespace WinFormsUI.Game.Player.PlayerStates
{
    public class WalkState : IPlayerState
    {
        public string DebugName => "Walking";

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

            var hor = player.HorizontalInput(scene.Input);
            if (hor == 0)
            {
                player.SwitchTo<IdleState>();
                return;
            }

            var vel = B2Bodies.b2Body_GetLinearVelocity(b2body.Id);
            var delta = MathUtils.MoveToward(vel.X, player.Stats.TopSpeed * hor, player.Stats.Acceleration * dt) - vel.X;
            B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(delta, 0), true);
            player.SetFacing(new(hor, 0));
        }
    }
}
