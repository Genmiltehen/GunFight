using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Box2D
{
    internal class Box2DOnGroundSystem : IGameSystem
    {
        public int Priority => 550;

        public bool IsEnabled { get; set; } = true;

        public void Update(GScene _scene, float _dt)
        {
            var sensorEvents = B2Worlds.b2World_GetSensorEvents(_scene.World.Id);

            for (int i = 0; i < sensorEvents.beginCount; i++)
            {
                var ev = sensorEvents.beginEvents[i];
                var bodyId = B2Shapes.b2Shape_GetBody(ev.sensorShapeId);
                var playerUserData = B2Bodies.b2Body_GetUserData(bodyId).GetRef<PlayerUserData>();
                if (playerUserData != null) playerUserData.GroundCollisions++;
            }
            for (int i = 0; i < sensorEvents.endCount; i++)
            {
                var ev = sensorEvents.endEvents[i];
                var bodyId = B2Shapes.b2Shape_GetBody(ev.sensorShapeId);
                var playerUserData = B2Bodies.b2Body_GetUserData(bodyId).GetRef<PlayerUserData>();
                if (playerUserData != null) playerUserData.GroundCollisions--;
            }
        }
    }
}
