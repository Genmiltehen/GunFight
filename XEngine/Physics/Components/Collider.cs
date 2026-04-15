using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Physics.Collision.Shapes;

namespace XEngine.Core.Physics.Components
{
    public class Collider : GameComponent, IDirty
    {
        public ICollider Shape { get; set; }
        public bool IsTrigger;

        public Collider Init(ICollider shape, Vector2 offset)
        {
            Shape = shape;
            Shape.Offset = offset;
            return this;
        }

        public int Layer { get; set; } = 1;
        public int CollisionMask { get; set; } = -1;

        public void SetDirty() => Shape.SetDirty();
        public Box2 GetBounds(TransformComp tr) => Shape.GetBounds(tr);
    }
}
