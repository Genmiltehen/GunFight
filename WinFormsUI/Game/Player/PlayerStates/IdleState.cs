using Box2D.NET;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;
using static WinFormsUI.Game.Player.PlayerControlHelper;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class IdleState : IPlayerState
    {
        public string DebugName => "Idling";

        public void Enter(GPlayer player, GScene scene)
        {
            if (player.HeadEntity.TryGet<GSprite>(out var hSprite)) hSprite
                    .SetTexture(scene.Assets.LoadTexture(player.HeadIdle), true);
            if (player.BodyEntity.TryGet<GSprite>(out var bSprite)) bSprite
                    .SetTexture(scene.Assets.LoadTexture(player.BodyIdle), true);
        }

        public void Exit(GPlayer player, GScene scene) { }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            var b2body = player.Owner.Get<GBox2DBody>()!;
            if (scene.Input.IsActionJustPressed($"jump{player.Name}"))
            {
                player.JumpTimer.Start();
                B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(0, player.Stats.JumpPower), true);
            }

            if (!IsOnGround(player))
            {
                player.SwitchTo<FallState>(scene);
                return;
            }

            if (HorizotnalInput(player, scene.Input) != 0)
            {
                player.SwitchTo<WalkState>(scene);
                return;
            }

            var shoot = scene.Input.IsActionActive($"shoot{player.Name}");
            if (shoot)
            {
                player.SwitchTo<AimState>(scene);
                return;
            }
        }
    }
}
