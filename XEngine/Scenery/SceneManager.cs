using XEngine.Core.Graphics.OpenGL;

namespace XEngine.Core.Scenery
{
    public sealed class SceneManager
    {
        private readonly AssetManager _assets;
        public Scene? CurrentScene { get; private set; }

        public SceneManager(AssetManager assets)
        {
            _assets = assets;
        }

        public void SwitchTo(Scene newScene)
        {
            CurrentScene?.Unload();
            _assets.UnloadSceneAssets();

            newScene.Load();
            CurrentScene = newScene;
        }

        public void Update(float dt)
        {
            CurrentScene?.Update(dt);
        }
    }
}
