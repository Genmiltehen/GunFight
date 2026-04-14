using OpenTK.Mathematics;
using System.Diagnostics;
using XEngine.Core.Base;
using XEngine.Core.Common;

namespace XEngine.Core.Graphics
{
    public sealed class CameraComp : GameComponent
    {
        public int Priority { get; private set; }

        private float _zoom = 0;
        private float _ezoom = 1;
        public TransformComp Transform => Owner.Get<TransformComp>()!;
        
        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;
                _ezoom = MathF.Exp(_zoom);
                Transform.Scale = new Vector2(_ezoom, _ezoom);
                Transform.SetDirty();
            }
        }

        public Matrix4 GetViewMatrix()
        {
            var worldMatrix = Transform.GetWorldMatrix();
            return Matrix4.Invert(worldMatrix);
        }
    }
}
