using Box2D.NET;
using System.Diagnostics;
using XEngine.Core.Base;
using XEngine.Core.Scenery;

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

        private static void PollContacts(GScene _scene)
        {
            var contact = B2Worlds.b2World_GetContactEvents(_scene.World.Id);
            for (int i = 0; i < contact.beginCount; i++)
            {
                var ev = contact.beginEvents[i];
                var bodyAId = B2Shapes.b2Shape_GetBody(ev.shapeIdA);
                var bodyBId = B2Shapes.b2Shape_GetBody(ev.shapeIdB);
                ContactWrapper cw = new(ev);
                var bodyA_UserData = B2Bodies.b2Body_GetUserData(bodyAId).GetRef<UserData>();
                bodyA_UserData?.HostBody.CollisionEnter(_scene, cw);
                var bodyB_UserData = B2Bodies.b2Body_GetUserData(bodyBId).GetRef<UserData>();
                bodyB_UserData?.HostBody.CollisionEnter(_scene, cw);
            }
            for (int i = 0; i < contact.endCount; i++)
            {
                var ev = contact.endEvents[i];
                var bodyAId = B2Shapes.b2Shape_GetBody(ev.shapeIdA);
                var bodyBId = B2Shapes.b2Shape_GetBody(ev.shapeIdB);
                ContactWrapper cw = new(ev);
                var bodyA_UserData = B2Bodies.b2Body_GetUserData(bodyAId).GetRef<UserData>();
                bodyA_UserData?.HostBody.CollisionExit(_scene, cw);
                var bodyB_UserData = B2Bodies.b2Body_GetUserData(bodyBId).GetRef<UserData>();
                bodyB_UserData?.HostBody.CollisionExit(_scene, cw);
            }
            for (int i = 0; i < contact.hitCount; i++)
            {
                var ev = contact.hitEvents[i];
                var bodyAId = B2Shapes.b2Shape_GetBody(ev.shapeIdA);
                var bodyBId = B2Shapes.b2Shape_GetBody(ev.shapeIdB);
                ContactWrapper cw = new(ev);
                var bodyA_UserData = B2Bodies.b2Body_GetUserData(bodyAId).GetRef<UserData>();
                bodyA_UserData?.HostBody.CollisionExit(_scene, cw);
                var bodyB_UserData = B2Bodies.b2Body_GetUserData(bodyBId).GetRef<UserData>();
                bodyB_UserData?.HostBody.CollisionExit(_scene, cw);
            }
        }
        private static void PollSensors(GScene _scene)
        {
            var contact = B2Worlds.b2World_GetSensorEvents(_scene.World.Id);
            for (int i = 0; i < contact.beginCount; i++)
            {
                var ev = contact.beginEvents[i];
                var bodyAId = B2Shapes.b2Shape_GetBody(ev.sensorShapeId);
                var bodyBId = B2Shapes.b2Shape_GetBody(ev.visitorShapeId);
                ContactWrapper cw = new(ev);
                var bodyA_UserData = B2Bodies.b2Body_GetUserData(bodyAId).GetRef<UserData>();
                bodyA_UserData?.HostBody.CollisionEnter(_scene, cw);
                var bodyB_UserData = B2Bodies.b2Body_GetUserData(bodyBId).GetRef<UserData>();
                bodyB_UserData?.HostBody.CollisionEnter(_scene, cw);
            }
            for (int i = 0; i < contact.endCount; i++)
            {
                var ev = contact.endEvents[i];
                var bodyAId = B2Shapes.b2Shape_GetBody(ev.sensorShapeId);
                var bodyBId = B2Shapes.b2Shape_GetBody(ev.visitorShapeId);
                ContactWrapper cw = new(ev);
                var bodyA_UserData = B2Bodies.b2Body_GetUserData(bodyAId).GetRef<UserData>();
                bodyA_UserData?.HostBody.CollisionExit(_scene, cw);
                var bodyB_UserData = B2Bodies.b2Body_GetUserData(bodyBId).GetRef<UserData>();
                bodyB_UserData?.HostBody.CollisionExit(_scene, cw);
            }
        }
    }
}
