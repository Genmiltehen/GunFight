using Box2D.NET;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.Maths;
using static WinFormsUI.Game.Player.PlayerControlHelper;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class FallState : IPlayerState
    {
        public string DebugName => "Falling";

        public void Enter(GPlayer player, GScene scene)
        {
            if (player.HeadEntity.TryGet<GSprite>(out var hSprite)) hSprite
                    .SetTexture(scene.Assets.LoadTexture(player.HeadIdle), true);
            if (player.BodyEntity.TryGet<GSprite>(out var bSprite)) bSprite
                    .SetTexture(scene.Assets.LoadTexture(player.BodyJump), true);
        }

        public void Exit(GPlayer player, GScene scene)
        {
            // TODO: animation end
        }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (IsOnGround(player))
            {
                if (player.JumpTimer.IsRunning) player.JumpTimer.Reset();
                player.SwitchTo<IdleState>(scene);
                return;
            }

            var hor = HorizotnalInput(player, scene.Input);
            var b2bodyId = player.Owner.Get<GBox2DBody>()!.Id;
            var vel = B2Bodies.b2Body_GetLinearVelocity(b2bodyId);
            var delta = MathUtils.MoveToward(vel.X, player.Stats.TopSpeed * hor, player.Stats.Acceleration * dt) - vel.X;
            B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2bodyId, new(delta * 0.5f, 0), true);
            if (hor != 0) PlayerHelper.SetFacing(player, new(hor, 0));

            
            var jump = JumpStart(player, scene.Input);
            if (!jump) player.JumpTimer.Reset();

            if (jump && player.JumpTimer.IsRunning)
                B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2bodyId, new(0, player.Stats.JumpPower * dt), true);

            if (scene.Input.IsActionJustReleased($"jump{player.Name}"))
                B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2bodyId, new(0, -vel.Y * 0.5f), true);
        }
    }
}
