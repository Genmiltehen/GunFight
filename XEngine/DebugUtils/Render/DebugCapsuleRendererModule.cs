using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Common;
using XEngine.Core.Graphics;
using XEngine.Core.Physics.Collision.Shapes;
using XEngine.Core.Physics.Components;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace XEngine.Core.DebugUtils.Render
{
    public class DebugCapsuleRendererModule : RenderModule
    {
        public override int Priority => 100;
        private readonly IGLContext _context;

        public DebugCapsuleRendererModule(IGLContext context)
        {
            _context = context;
        }

        public override void Render(Scene scene)
        {
            var shader = _context.GetShader("CapsuleDebug");
            shader.Use();

            shader.SetMatrix4("uProjection", _projection);
            shader.SetMatrix4("uView", scene.Camera.GetViewMatrix());
            shader.SetVector3("uColor", new Vector3(1.0f, 0.0f, 0.0f));

            _context.UnitQuad.Bind();

            foreach (var (_, tr, cc) in scene.Query<TransformComp, Collider>())
            {
                if (cc.Shape is not CapsuleCollider col) continue;

                Segment seg = col.GetSegment(tr);
                shader.SetVector2("uPointA", seg.start);
                shader.SetVector2("uPointB", seg.end);

                shader.SetFloat("uRadius", col.Radius * tr.Scale.X);

                Box2 aabbCalsule = col.GetBounds(tr);
                Matrix4 modelMatrix =
                    Matrix4.CreateScale(aabbCalsule.Size.X, aabbCalsule.Size.Y, 1.0f) *
                    Matrix4.CreateTranslation(tr.Position2D.X, tr.Position2D.Y, 0.0f);
                shader.SetMatrix4("uModel", modelMatrix);

                _context.UnitQuad.Draw();
            }

            GL.BindVertexArray(0);
        }
    }
}
