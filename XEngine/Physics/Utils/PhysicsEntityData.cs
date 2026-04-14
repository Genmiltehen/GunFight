using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Common;

namespace XEngine.Core.Physics.Utils
{
    public struct PhysicsEntityData
    {
        public int entityId;
        public RigidbodyComp rb;
        public TransformComp tr;
        public ColliderComp col;
    }
}
