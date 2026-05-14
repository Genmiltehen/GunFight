using WinFormsUI.Game.Player;
using WinFormsUI.Game.Scenes;
using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game
{
    internal class WinSystem : IGameSystem
    {
        private bool IsWon = false;
        public int Priority => 700;

        public bool IsEnabled { get; set; } = true;

        public void Update(GScene _scene, float _dt)
        {
            if (!IsWon && _scene is MainScene scene)
            {
                var e = scene.Query<GPlayer>();
                if (e.Count() == 1)
                {
                    IsWon = true;
                    scene.WinnerName = e.FirstOrDefault().Item2.Name;
                    scene.End();
                }
            }
        }
    }
}
