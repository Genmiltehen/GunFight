using Box2D.NET;
using System.Diagnostics;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Input;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Player
{
    internal class PlayersSystem(IInputService input) : InputSystem(input)
    {
        public override void Update(GScene _scene, float _dt)
        {
            foreach (var (_, player) in _scene.Query<GPlayer>())
            {
                player.ProcessInput(_scene, _dt);
            }
        }
    }
}
