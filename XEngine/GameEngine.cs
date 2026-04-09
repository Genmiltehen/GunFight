using System.IO;
using System.Text.Json;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Config;
using XEngine.Core.Graphics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Input;
using XEngine.Core.Scenery;

namespace XEngine.Core
{
    public class GameEngine
    {
        private readonly AssetManager _assets;
        private readonly SceneManager _sceneManager;
        private readonly RenderPipeline _renderPipeline;
        private readonly InputService _input;

        public AssetManager Assets => _assets;
        public SceneManager SceneManager => _sceneManager;
        public RenderPipeline Renderer => _renderPipeline;
        public InputService Input => _input;

        public GameConfig _config;

        public GameEngine(string AssetPath)
        {
            _assets = new AssetManager(AssetPath);
            _sceneManager = new SceneManager(_assets);
            _renderPipeline = new RenderPipeline();
            _input = new InputService();
            _config = new ConfigManager().Load(Path.Combine(AssetPath, "config.json"));
        }

        public void Init()
        {
            _assets.Init();

            var spriteRenderer = new SpriteRendererModule(_assets);
            _renderPipeline.AddRenderModule(spriteRenderer);

            _input.LoadBindingsFromConfig(_config);
        }

        public void Update(float dt)
        {
            _sceneManager.Update(dt);
            _input.Update(dt);
        }

        public void Render()
        {
            if (_sceneManager.CurrentScene != null) _renderPipeline.Render(_sceneManager.CurrentScene);
        }
    }
}
