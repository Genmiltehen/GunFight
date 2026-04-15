using XEngine.Core.Base;
using XEngine.Core.Physics.Collision.NarrowPhase;
using XEngine.Core.Physics.MathStructs;
using XEngine.Core.Scenery;

namespace XEngine.Core.Physics.Collision.Detection
{
    public interface IBroadPhase
    {
        IEnumerable<(CollisionObject, CollisionObject)> GetPotentialPairs(Scene scene);
    }
}
