using OpenTK.Mathematics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace XEngine.Core.Graphics
{
    public abstract class RenderModule(GLProvider provider)
    {
        protected Vector2 _screenSize;
        protected readonly GLProvider _provider = provider;

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
