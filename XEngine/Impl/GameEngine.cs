using GameEngineLib.Defaults;
using GameEngineLib.Defaults.Render;
using GameEngineLib.Impl.OpenGl;
using GameEngineLib.Impl.RenderImpl;
using GameEngineLib.Impl.SceneImpl;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace GameEngineLib.Impl
{
    public class GameEngine
    {
        private AssetManager _assets;
        private SceneManager _sceneManager;
        private RenderPipeline _renderPipeline;

        public AssetManager Assets => _assets;
        public RenderPipeline Renderer => _renderPipeline;
        public SceneManager SceneManager => _sceneManager;

        public GameEngine() { }

        public void Init(string AssetPath)
        {
            _assets = new AssetManager(AssetPath);
            _sceneManager = new SceneManager(_assets);
            _renderPipeline = new RenderPipeline();

            var _r = new RendererSystem(_assets);
            _renderPipeline.AddRenderer(_r);
        }

        public void AddCamera()
        {
            var _cam = _sceneManager.CurrentScene!.CreateEntity("MainCamera");
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
