using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class AirborneState : IPlayerState
    {
        public string DebugName => "Falling";

        public void Enter(GPlayer player)
        {
            // TODO: animation
        }

        public void Exit(GPlayer player)
        {
            // TODO: animation end
        }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (player.IsOnGround()) player.SwitchTo<IdleState>();

            var hor = player.HorizontalInput(scene.Input);
            var b2body = player.Owner.Get<GBox2DBody>()!;
            var vel = B2Bodies.b2Body_GetLinearVelocity(b2body.Id);
            var delta = Box2DMathUtils.MoveToward(vel.X, player.TopSpeed * hor, player.Acceleration * dt) - vel.X;
            B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(delta * 0.5f, 0), true);
        }
    }
}
