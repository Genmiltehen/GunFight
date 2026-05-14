using XEngine.Core.Input;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Player
{
    internal class PlayersSystem(IInputService input) : InputSystem(input)
    {
        public override void Update(GScene _scene, float _dt)
        {
            foreach (var (_, player) in _scene.Query<GPlayer>())
            {
                player.ProcessInput(_scene, _dt);
                if (player.Owner.Transform.Position.Y < -15) player.Owner.MarkDelete();
            }
        }
    }
}
