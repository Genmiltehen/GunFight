using OpenTK.Graphics.OpenGL4;
using XEngine.Core.Scenery;


namespace XEngine.Core.Graphics
{
    public class RenderPipeline
    {
        private readonly List<IRenderModule> _renderModules = [];

        public void AddRenderModule(IRenderModule renderModule)
        {
            _renderModules.Add(renderModule);
            _renderModules.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void SetViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            foreach (var _r in _renderModules) _r.OnResize(width, height);
        }

        public void Render(Scene scene)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (var _r in _renderModules) if (_r.IsEnabled) _r.Render(scene);
        }
    }
}
