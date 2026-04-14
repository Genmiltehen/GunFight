using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;

namespace XEngine.Core.Physics.Utils
{
    public struct CollisionManifold
    {
        public PhysicsEntityData physA;
        public PhysicsEntityData physB;

        public Vector2 Normal; // A -> B
        public ContactPoint[] contacts;
    }
}
