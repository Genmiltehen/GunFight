using Box2D.NET;
using XEngine.Core.Base;
using XEngine.Core.Scenery;
using static Box2D.NET.B2Shapes;
using static Box2D.NET.B2Bodies;
using System.Diagnostics;

namespace XEngine.Core.Box2DCompat.Systems
{
    public class Box2DContactSystem : IGameSystem
    {
        public int Priority => 50;

        public bool IsEnabled { get; set; } = true;

        public void Update(GScene _scene, float _dt)
        {
            PollContacts(_scene);
            PollSensors(_scene);
        }

        private static void Enter(GScene scene, B2ShapeId shapeId, ContactWrapper cw)
        {
            b2Body_GetUserData(b2Shape_GetBody(shapeId)).GetRef<UserData>()
                ?.HostBody.CollisionEnter(scene, cw);
        }

        private static void Exit(GScene scene, B2ShapeId shapeId, ContactWrapper cw)
        {
            b2Body_GetUserData(b2Shape_GetBody(shapeId)).GetRef<UserData>()
                ?.HostBody.CollisionExit(scene, cw);
        }

        private static void PollContacts(GScene _scene)
        {
            var contact = B2Worlds.b2World_GetContactEvents(_scene.World.Id);
            for (int i = 0; i < contact.beginCount; i++)
            {
                var ev = contact.beginEvents[i];
                Enter(_scene, ev.shapeIdA, new(ev));
                Enter(_scene, ev.shapeIdB, new(ev));
            }
            for (int i = 0; i < contact.endCount; i++)
            {
                var ev = contact.endEvents[i];
                Exit(_scene, ev.shapeIdA, new(ev));
                Exit(_scene, ev.shapeIdB, new(ev));
            }
        }

        private static void PollSensors(GScene _scene)
        {
            var contact = B2Worlds.b2World_GetSensorEvents(_scene.World.Id);
            for (int i = 0; i < contact.beginCount; i++)
            {
                var ev = contact.beginEvents[i];
                Enter(_scene, ev.sensorShapeId, new(ev));
                Enter(_scene, ev.visitorShapeId, new(ev));
            }
            for (int i = 0; i < contact.endCount; i++)
            {
                var ev = contact.endEvents[i];
                Exit(_scene, ev.sensorShapeId, new(ev));
                Exit(_scene, ev.visitorShapeId, new(ev));
            }
        }
    }
}
