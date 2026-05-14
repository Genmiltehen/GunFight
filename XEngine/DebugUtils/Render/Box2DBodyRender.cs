using OpenTK.Graphics.OpenGL4;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Graphics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace XEngine.Core.DebugUtils.Render
{
    public class Box2DBodyRender(GLProvider glProvider) : RenderModule(glProvider)
    {
        public override int Priority => 0;

        public override void Render(GScene scene)
        {
            var _shader = _provider.GetShader("Line");

            _shader.Use();

            _shader.SetMatrix4("uProjection", scene.Camera.GetProjectionMatrix(_screenSize));
            _shader.SetMatrix4("uView", scene.Camera.GetViewMatrix());

            foreach (var (_, b2b) in scene.Query<GBox2DBody>()) Box2DObjectTracer.TraceBody(b2b.Id, _provider.LineBatcher, new(1, 0, 0, 1));

            _provider.LineBatcher.Draw();
            _provider.LineBatcher.Clear();
            GL.BindVertexArray(0);
        }
    }
}
