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
    public class WalkState : IPlayerState
    {
        public string DebugName => "Walking";

        public void Enter(GPlayer player) { }

        public void Exit(GPlayer player) { }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            var b2body = player.Owner.Get<GBox2DBody>()!;
            var jumped = scene.Input.IsActionJustPressed($"jump{player.Name}");
            if (jumped) B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(0, player.JumpPower), true);

            if (!player.IsOnGround())
            {
                player.SwitchTo<AirborneState>();
                return;
            }

            var hor = player.HorizontalInput(scene.Input);
            if (hor == 0)
            {
                player.SwitchTo<IdleState>();
                return;
            }

            var vel = B2Bodies.b2Body_GetLinearVelocity(b2body.Id);
            var delta = Box2DMathUtils.MoveToward(vel.X, player.TopSpeed * hor, player.Acceleration * dt) - vel.X;
            B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(delta, 0), true);
            player.SetFacing(new(hor, 0));
        }
    }
}
