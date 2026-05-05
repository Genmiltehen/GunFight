using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Graphics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace XEngine.Core.Common.Trace
{
    public class TracerRenderModule : RenderModule
    {
        public override int Priority => -1;
        private readonly GLProvider _glProvider;
        private readonly Shader _line_shader;

        public TracerRenderModule(GLProvider glProvider)
        {
            _glProvider = glProvider;
            _line_shader = _glProvider.GetShader("Line");
        }

        public override void Render(GScene scene)
        {
            if (_screenSize.X <= 0 || _screenSize.Y <= 0) return;

            _line_shader.Use();

            _line_shader.SetMatrix4("uProjection", scene.Camera.GetProjectionMatrix(_screenSize));
            _line_shader.SetMatrix4("uView", scene.Camera.GetViewMatrix());
            var lb = _glProvider.LineBatcher;
            foreach (var (_, tr) in scene.Query<GTrace>())
            {
                float brightness = 1;
                using (lb.TraceLine(closed: false))
                {
                    foreach (var p in tr.PointQueue.Reverse())
                    {
                        lb.AddPoint(new(p.X, p.Y), new(tr.Color.X, tr.Color.Y, tr.Color.Z, brightness));
                        brightness *= 0.2f;
                    }
                }
            }

            _glProvider.LineBatcher.Draw();
            _glProvider.LineBatcher.Clear();
            GL.BindVertexArray(0);
        }
    }
}
