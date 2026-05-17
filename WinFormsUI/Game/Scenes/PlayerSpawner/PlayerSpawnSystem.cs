using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes.PlayerSpawner
{
    public class PlayerSpawnSystem(string AId, string BId) : IGameSystem
    {
        private bool _AUnset = true;
        private bool _BUnset = true;

        public int Priority => -1;

        public bool IsEnabled { get; set; } = true;

        public void Update(GScene _scene, float _dt)
        {
            foreach (var (_, s) in _scene.Query<GPlayerSpawner>())
            {
                if (_AUnset && s.Name == "A") { _scene.Schedule(() => s.Spawn(AId)); _AUnset = false; }
                if (_BUnset && s.Name == "B") { _scene.Schedule(() => s.Spawn(BId)); _BUnset = false; }
            }
            _scene.Schedule(() => _scene.RemoveSystem(this));
        }
    }
}
