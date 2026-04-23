using OpenTK.Mathematics;
using XEngine.Core.Scenery;

namespace XEngine.Core.Graphics
{
    public abstract class RenderModule
    {
        protected int _screenWidth;
        protected int _screenHeight;
        protected Matrix4 _projection;

        public bool IsEnabled { get; set; } = true;
        public abstract int Priority { get; }

        public abstract void Render(GScene scene);
        public void OnResize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
            float halfW = _screenWidth / 2f;
            float halfH = _screenHeight / 2f;
            _projection = Matrix4.CreateOrthographicOffCenter(-halfW, halfW, -halfH, halfH, -1f, 1f);
        }
    }
}
