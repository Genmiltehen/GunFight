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
            if (!player.IsOnGround()) player.SwitchTo<AirborneState>();
            if (player.HorizontalInput(scene.Input) != 0) player.SwitchTo<WalkState>();

            var jumped = scene.Input.IsActionJustActivated($"jump{player.Name}");
            var b2body = player.Owner.Get<GBox2DBody>()!;
            if (jumped) B2Bodies.b2Body_ApplyLinearImpulseToCenter(b2body.Id, new(0, 20), true);
        }
    }
}
