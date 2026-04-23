using OpenTK.Mathematics;
using System.Diagnostics;
using XEngine.Core.Base;
using XEngine.Core.Common;

namespace XEngine.Core.Graphics
{
    public sealed class GCamera : GameComponent
    {
        public int Priority { get; private set; }
        public GTransform Transform => Owner.Get<GTransform>()!;

        private float _zoom = 0;
        private float _ezoom = 1;
        private bool _isDirty = true;
        private Matrix4 _savedScale = Matrix4.CreateScale(1, 1, 1);

        public float EZoom => _ezoom;
        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;
                _ezoom = MathF.Exp(_zoom);
                _isDirty = true;
            }
        }

        public Matrix4 GetViewMatrix()
        {
            if (_isDirty)
            {
                _savedScale = Matrix4.CreateScale(_ezoom, _ezoom, 1);
                _isDirty = false;
            }
            var worldMatrix = _savedScale * Transform.GetWorldMatrix();
            return Matrix4.Invert(worldMatrix);
        }
    }
}
