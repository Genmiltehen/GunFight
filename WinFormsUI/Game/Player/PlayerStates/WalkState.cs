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
            if (!player.IsOnGround()) player.SwitchTo<AirborneState>();

            var hor = player.HorizontalInput(scene.Input);
            if (hor == 0) player.SwitchTo<IdleState>();

            var b2body = player.Owner.Get<GBox2DBody>()!;
            var jumped = scene.Input.IsActionJustActivated($"jump{player.Name}");
            if (jumped) B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(0, 20), true);

            var vel = B2Bodies.b2Body_GetLinearVelocity(b2body.Id);
            var delta = Box2DMathUtils.MoveToward(vel.X, player.TopSpeed * hor, player.Acceleration * dt) - vel.X;
            B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(delta, 0), true);
        }
    }
}
