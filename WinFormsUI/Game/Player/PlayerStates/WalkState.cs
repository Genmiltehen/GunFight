using Box2D.NET;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;
using XEngine.Core.Utils.Maths;
using static WinFormsUI.Game.Player.PlayerControlHelper;

namespace WinFormsUI.Game.Player.PlayerStates
{
    public class WalkState : IPlayerState
    {
        public string DebugName => "Walking";
        private int Frame = 0;

        public void Enter(GPlayer player, GScene scene)
        {
            if (player.HeadEntity.TryGet<GSprite>(out var hSprite)) hSprite
                    .SetTexture(scene.Assets.LoadTexture(player.HeadMove), true);
            player.WalkTimer.Start();
        }

        public void Exit(GPlayer player, GScene scene) { }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            var b2body = player.Owner.Get<GBox2DBody>()!;

            if (JumpStart(player, scene.Input))
            {
                player.JumpTimer.Start();
                B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(0, player.Stats.JumpPower), true);
            }

            if (!IsOnGround(player))
            {
                player.SwitchTo<FallState>(scene);
                return;
            }

            var hor = HorizotnalInput(player, scene.Input);
            if (hor == 0)
            {
                player.SwitchTo<IdleState>(scene);
                return;
            }

            if (!player.WalkTimer.IsRunning)
            {
                UpdateFrame(player, scene);
                player.WalkTimer.Start();
            }

            var vel = B2Bodies.b2Body_GetLinearVelocity(b2body.Id);
            var delta = MathUtils.MoveToward(vel.X, player.Stats.TopSpeed * hor, player.Stats.Acceleration * dt) - vel.X;
            B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(delta, 0), true);
            PlayerHelper.SetFacing(player, new(hor, 0));
        }

        private void UpdateFrame(GPlayer player, GScene scene)
        {
            Frame = (Frame + 1) % 4;
            var frameName = Frame switch
            {
                0 => player.BodyMoveM,
                1 => player.BodyMoveL,
                2 => player.BodyMoveM,
                3 => player.BodyMoveR,
                _ => ""
            };

            if (player.BodyEntity.TryGet<GSprite>(out var bSprite)) bSprite
                    .SetTexture(scene.Assets.LoadTexture(frameName), true);
        }
    }
}
