using OpenTK.Mathematics;
using System.Diagnostics;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Scenery;

namespace XEngine.Core.Graphics
{
    public sealed class GCamera : GameComponent
    {
        public int Priority { get; private set; }

        private float _zoom = 1;
        private Matrix4 _savedScale = Matrix4.CreateScale(1, 1, 1);

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (value != _zoom)
                {
                    _zoom = value;
                    _savedScale = Matrix4.CreateScale(_zoom, _zoom, 1);
                }
            }
        }

        public void Approach(Vector3 newPos, float strength)
        {
            Owner.Transform.Position = Vector3.Lerp(Owner.Transform.Position, newPos, strength);
        }

        public Matrix4 GetProjectionMatrix(Vector2 size)
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4, size.X / size.Y, 1, 200);
        }

        public Matrix4 GetViewMatrix()
        {
            return _savedScale * Matrix4.Invert(Owner.Transform.GetWorldMatrix());
        }
    }
}
