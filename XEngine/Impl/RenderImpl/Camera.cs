using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineLib.Impl.RenderImpl
{
    public class Camera
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Zoom { get; set; } = 1.0f;

        public Matrix4 GetViewMatrix()
        {
            var translation = Matrix4.CreateTranslation(-Position.X, -Position.Y, 0);
            var scale = Matrix4.CreateScale(Zoom, Zoom, 1.0f);
            return translation * scale;
        }
    }
}
