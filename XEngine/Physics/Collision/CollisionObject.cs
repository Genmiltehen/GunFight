using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Physics.Components;

namespace XEngine.Core.Physics.Collision
{
    public struct CollisionObject
    {
        public int entityId;
        public Rigidbody rb;
        public TransformComp tr;
        public Collider col;
    }
}
