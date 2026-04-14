using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEngine.Core.Physics.Utils
{
    public delegate CollisionManifold? CollisionChecker(PhysicsEntityData a, PhysicsEntityData b);
}
