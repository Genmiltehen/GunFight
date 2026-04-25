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
    internal class FallState : IPlayerState
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
            if (player.IsOnGround())
            {
                if (player.JumpTimer.IsRunning) player.JumpTimer.Reset();
                player.SwitchTo<IdleState>();
                return;
            }

            var hor = player.HorizontalInput(scene.Input);
            var b2bodyId = player.Owner.Get<GBox2DBody>()!.Id;
            var vel = B2Bodies.b2Body_GetLinearVelocity(b2bodyId);
            var delta = MathUtils.MoveToward(vel.X, player.Stats.TopSpeed * hor, player.Stats.Acceleration * dt) - vel.X;
            B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2bodyId, new(delta * 0.5f, 0), true);
            if (hor != 0) player.SetFacing(new(hor, 0));

            var jump = scene.Input.IsActionActive($"jump{player.Name}");
            if (!jump) player.JumpTimer.Reset();

            if (jump && player.JumpTimer.IsRunning)
                B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2bodyId, new(0, player.Stats.JumpPower * dt), true);

            if (scene.Input.IsActionJustReleased($"jump{player.Name}"))
                B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2bodyId, new(0, -vel.Y * 0.5f), true);
        }
    }
}
