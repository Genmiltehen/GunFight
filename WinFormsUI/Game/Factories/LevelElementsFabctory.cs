using Box2D.NET;
using WinFormsUI.Game.Box2D;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Factories
{
    internal static class LevelElementsFabctory
    {
        public static Entity CreatePlatform(GScene scene, B2Vec2 pos, B2Vec2 size, float rotation = 0)
        {
            var ground = scene.CreateEntity();
            var tr = ground.AddComponent<GTransform>().Init(new(pos.X, pos.Y, 0), rotation);
            ground.AddComponent<GBox2DBody>()
                .Init()
                .SetType(B2BodyType.b2_staticBody)
                .SyncToTransform(tr)
                .Build(scene.World.Id)
                .AttacShapes(bid =>
                {
                    B2Polygon groundBox = B2Geometries.b2MakeBox(size.X / 2, size.Y / 2);
                    B2ShapeDef groundShapeDef = B2Types.b2DefaultShapeDef();
                    groundShapeDef.material.friction = 1;
                    groundShapeDef.enableSensorEvents = true;
                    groundShapeDef.filter.categoryBits = (ulong)CollisionFlags.GROUND;
                    groundShapeDef.filter.maskBits = (ulong)(CollisionFlags.FOOT | CollisionFlags.PLAYER);
                    B2Shapes.b2CreatePolygonShape(bid, groundShapeDef, groundBox);
                });
            return ground;
        }
    }
}
