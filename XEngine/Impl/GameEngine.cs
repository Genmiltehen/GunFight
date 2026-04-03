using GameEngineLib.Impl.OpenGl;
using GameEngineLib.Impl.RenderImpl;
using GameEngineLib.Impl.SceneImpl;
using OpenTK.Graphics.OpenGL4;

namespace GameEngineLib.Impl
{
    public class GameEngine
    {
        private readonly AssetManager _assets;
        private readonly SceneManager _sceneManager;
        private readonly RenderPipeline _renderPipeline;

        public GameEngine(string AssetPath)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            _assets = new AssetManager(AssetPath);
            _sceneManager = new SceneManager(_assets);
            _renderPipeline = new RenderPipeline();
        }

        public void Update(float dt)
        {
            _sceneManager.Update(dt);
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _render
            _sceneManager.Render();
        }
    }
}
