using GameEngineLib.Impl.OpenGl;
using GameEngineLib.Impl.SceneImpl;
using OpenTK.Graphics.OpenGL4;

namespace GameEngineLib.Impl
{
    public class GameEngine
    {
        private readonly AssetManager _assets;
        private readonly SceneManager _sceneManager;

        public GameEngine(string AssetPath) {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            _assets = new AssetManager(AssetPath);
            _sceneManager = new SceneManager(_assets);
        }
    }   
}
