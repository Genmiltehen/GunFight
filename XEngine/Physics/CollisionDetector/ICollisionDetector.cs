using XEngine.Core.Base;
using XEngine.Core.Physics.Utils;
using XEngine.Core.Scenery;

namespace XEngine.Core.Physics.CollisionDetector
{
    public interface ICollisionDetector
    {
        IEnumerable<CollisionManifold> GetManifolds(Scene scene, CollisionChecker OnPairFound);
    }
}
