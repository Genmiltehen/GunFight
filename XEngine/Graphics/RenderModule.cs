using OpenTK.Mathematics;
using XEngine.Core.Scenery;

namespace XEngine.Core.Graphics
{
    public abstract class RenderModule
    {
        protected Vector2 _screenSize;

        public bool IsEnabled { get; set; } = true;
        public abstract int Priority { get; }

        public abstract void Render(GScene scene);
        public void OnResize(int width, int height)
        {
            _screenSize.X = width;
            _screenSize.Y = height;
        }
    }
}
