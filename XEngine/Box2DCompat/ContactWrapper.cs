using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace XEngine.Core.Box2DCompat
{
    public readonly struct ContactWrapper
    {
        private readonly B2ShapeId _shapeIdA;
        private readonly B2ShapeId _shapeIdB;
        private readonly B2ContactId _contactId;
        public readonly bool IsSensor;

        public B2ContactId? ContactId => IsSensor ? null : _contactId;
        public B2ShapeId ShapeIdA => _shapeIdA;
        public B2ShapeId ShapeIdB => _shapeIdB;
        public B2BodyId BodyIdA => B2Shapes.b2Shape_GetBody(_shapeIdA);
        public B2BodyId BodyIdB => B2Shapes.b2Shape_GetBody(_shapeIdB);
        public GBox2DBody? GBodyA => B2Bodies.b2Body_GetUserData(BodyIdA).GetRef<UserData>()?.HostBody;
        public GBox2DBody? GBodyB => B2Bodies.b2Body_GetUserData(BodyIdB).GetRef<UserData>()?.HostBody;
        public Entity? EntityA => B2Bodies.b2Body_GetUserData(BodyIdA).GetRef<UserData>()?.HostBody.Owner;
        public Entity? EntityB => B2Bodies.b2Body_GetUserData(BodyIdB).GetRef<UserData>()?.HostBody.Owner;

        public ContactWrapper(B2ContactBeginTouchEvent ev)
        {
            _shapeIdA = ev.shapeIdA;
            _shapeIdB = ev.shapeIdB;
            _contactId = ev.contactId;
            IsSensor = false;
        }

        public ContactWrapper(B2ContactEndTouchEvent ev)
        {
            _shapeIdA = ev.shapeIdA;
            _shapeIdB = ev.shapeIdB;
            _contactId = ev.contactId;
            IsSensor = false;
        }

        public ContactWrapper(B2ContactHitEvent ev)
        {
            _shapeIdA = ev.shapeIdA;
            _shapeIdB = ev.shapeIdB;
            _contactId = ev.contactId;
            IsSensor = false;
        }

        public ContactWrapper(B2SensorBeginTouchEvent ev)
        {
            _shapeIdA = ev.sensorShapeId;
            _shapeIdB = ev.visitorShapeId;
            IsSensor = true;
        }

        public ContactWrapper(B2SensorEndTouchEvent ev)
        {
            _shapeIdA = ev.sensorShapeId;
            _shapeIdB = ev.visitorShapeId;
            IsSensor = true;
        }
    }
}
