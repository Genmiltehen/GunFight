using GameEngineLib.Impl.SceneImpl;

namespace GameEngineLib.Impl.RenderImpl
{
    public interface IRenderSystem
    {
        public bool IsEnabled { get; set; }
        public int Priority { get; }
        void Render(Scene scene, CameraComp camera);
        void OnResize(int width, int height);
    }
}
