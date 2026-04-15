using OpenTK.Mathematics;
using XEngine.Core.Common;

namespace XEngine.Core.Physics.Collision.Shapes
{
    public interface ICollider : IDirty
    {
        Vector2 Offset { get; set; }
        ColliderType ColliderType { get; }
        Box2 GetBounds(TransformComp tr);
        float UnitMassMoI { get; }
    }
}
