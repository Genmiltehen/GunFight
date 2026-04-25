using Box2D.NET;
using OpenTK.Mathematics;
using System.Diagnostics;
using XEngine.Core.Box2DCompat;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Graphics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace XEngine.Core.DebugUtils.Render
{
    public class Box2DBodyRender : RenderModule
    {
        public override int Priority => 0;
        private readonly GLProvider _glProvider;
        private readonly Shader _line_shader;

        public Box2DBodyRender(GLProvider glProvider)
        {
            _glProvider = glProvider;
            _line_shader = _glProvider.GetShader("Line");
        }

        public override void Render(GScene scene)
        {
            _line_shader.Use();

            _line_shader.SetMatrix4("uProjection", scene.Camera.GetProjectionMatrix(_screenSize));
            _line_shader.SetMatrix4("uView", scene.Camera.GetViewMatrix());
            _line_shader.SetVector3("uColor", new(1, 0, 0));

            foreach (var (_, b2b) in scene.Query<GBox2DBody>())
            {
                Box2DObjectTracer.TraceBody(b2b.Id, _glProvider.LineBatcher);
            }

            _glProvider.LineBatcher.Draw();
            _glProvider.LineBatcher.Clear();
        }
    }
}
