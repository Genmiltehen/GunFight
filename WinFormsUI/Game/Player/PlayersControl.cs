using Box2D.NET;
using System.Diagnostics;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Input;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Player
{
    internal class PlayersControl(IInputService input) : InputSystem(input)
    {
        private float playerTopSpeed = 5;
        private float playerAccel = 30;

        public override void Update(GScene _scene, float _dt)
        {
            var (eA, playerA) = _scene.Query<GPlayer>(e => e.Has<PlayerATag>()).FirstOrDefault();
            playerA.ProcessInput(_scene, _dt);
        }
    }
}
