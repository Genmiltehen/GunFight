using XEngine.Core.Defaults.Sprite;
using XEngine.Core.Defaults;
using XEngine.Core.Graphics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace XEngine.Core
{
    public class GameEngine
    {
        private readonly AssetManager _assets;
        private readonly SceneManager _sceneManager;
        private readonly RenderPipeline _renderPipeline;

        public AssetManager Assets => _assets;
        public RenderPipeline Renderer => _renderPipeline;
        public SceneManager SceneManager => _sceneManager;

        public GameEngine(string AssetPath)
        {
            _assets = new AssetManager(AssetPath);
            _sceneManager = new SceneManager(_assets);
            _renderPipeline = new RenderPipeline();
        }

        public void Init()
        {
            _assets.Init();

            var _r = new SpriteRendererModule(_assets);
            _renderPipeline.AddRenderModule(_r);
        }

        public void AddCamera()
        {
            var _cam = _sceneManager.CurrentScene!.CreateEntity();
            _cam.AddComponent<TransformComp>();
            _cam.AddComponent<CameraComp>().Init(_renderPipeline, 0);
        }

        public void Update(float dt)
        {
            _sceneManager.Update(dt);
        }

        public void Render()
        {
            if (_sceneManager.CurrentScene != null)
                _renderPipeline.Render(_sceneManager.CurrentScene);
        }
    }
}
