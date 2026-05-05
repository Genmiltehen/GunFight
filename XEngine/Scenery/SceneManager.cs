using XEngine.Core.Graphics;

namespace XEngine.Core.Scenery
{
    public sealed class SceneManager(AssetManager assets) : IDisposable
    {
        private bool _disposed = false;
        public GScene? CurrentScene { get; private set; }

        public void SwitchTo(GScene newScene)
        {
            CurrentScene?.Unload();
            assets.UnloadSceneAssets();

            newScene.Load();
            CurrentScene = newScene;
        }

        public void Update(float dt)
        {
            CurrentScene?.Update(dt);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            CurrentScene?.Unload();
        }
    }
}
