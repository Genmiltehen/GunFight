using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Common;

namespace XEngine.Core.Graphics
{
    public sealed class CameraComp : GameComponent
    {
        public int Priority { get; private set; }

        public TransformComp Transform => Owner.Get<TransformComp>()!;

        public Matrix4 GetViewMatrix()
        {
            var worldMatrix = Transform.GetMatrix();
            return Matrix4.Invert(worldMatrix);
        }
    }
}
