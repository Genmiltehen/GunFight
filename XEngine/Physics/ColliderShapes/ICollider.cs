using OpenTK.Mathematics;
using XEngine.Core.Common;
using XEngine.Core.Physics.Utils;

namespace XEngine.Core.Physics.ColliderShapes
{
    public interface ICollider : IDirty
    {
        Vector2 Offset { get; set; }
        ColliderType ColliderType { get; }
        Box2 GetBounds(TransformComp tr);
        float UnitMassMoI { get; }
    }
}
