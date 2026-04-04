using XEngine.Core.Scenery;

namespace XEngine.Core.Graphics
{
    public interface IRenderModule
    {
        public bool IsEnabled { get; set; }
        public int Priority { get; }
        void Render(Scene scene, CameraComp camera);
        void OnResize(int width, int height);
    }
}
