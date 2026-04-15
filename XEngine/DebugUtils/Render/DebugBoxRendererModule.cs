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
    public class DebugBoxRendererModule : RenderModule
    {
        public override int Priority => 100;
        private readonly IGLContext _context;

        public DebugBoxRendererModule(IGLContext context)
        {
            _context = context;
        }

        public override void Render(Scene scene)
        {
            var shader = _context.GetShader("BoxDebug");
            shader.Use();

            shader.SetMatrix4("uProjection", _projection);
            shader.SetMatrix4("uView", scene.Camera.GetViewMatrix());
            shader.SetVector3("uColor", new Vector3(1.0f, 0.0f, 0.0f));

            _context.UnitQuad.Bind();

            foreach (var (_, tr, cc) in scene.Query<TransformComp, Collider>())
            {
                if (cc.Shape is not BoxCollider col) continue;

                shader.SetVector2("uCenter", tr.Position2D + col.Offset);
                shader.SetVector2("uSize", col.HalfSize * tr.Scale);
                shader.SetFloat("uAngle", tr.Rotation);
                //shader.SetFloat("uAngle", 0);

                Box2 aabbCalsule = col.GetBounds(tr);
                Matrix4 assetMatrix = Matrix4.CreateScale(aabbCalsule.Size.X + 5, aabbCalsule.Size.Y + 5, 1.0f);
                Matrix4 worldMatrix = Matrix4.CreateTranslation(tr.Position);
                shader.SetMatrix4("uModel", assetMatrix * worldMatrix);

                _context.UnitQuad.Draw();
            }

            GL.BindVertexArray(0);
        }
    }
}
