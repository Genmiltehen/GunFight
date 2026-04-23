using Box2D.NET;
using WinFormsUI.Game.Box2D;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Player
{
    public static class PlayerFactory
    {
        public static GPlayer CreatePlayer(GScene scene, B2Vec2 size)
        {
            var body = CreateBody(scene, size);
            var head = CreateHead(scene, size);
            var weapon = CreateWeapon(scene, size);
            head.Get<GTransform>()!.SetParent(body.Get<GTransform>()!);
            weapon.Get<GTransform>()!.SetParent(body.Get<GTransform>()!);

            return body.AddComponent<GPlayer>().Init(body, head, weapon);
        }

        private static Entity CreateBody(GScene scene, B2Vec2 size)
        {
            var e = scene.CreateEntity();
            var tr = e.AddComponent<GTransform>().Init(new(0, 10, 0), 0f);
            e.AddComponent<GSprite>()
                .SetScale(1.0f / scene.World.PixelPerMetre)
                .SetTranslation(new(0, size.Y * 0.5f));

            float r = 0.5f * size.X;
            var bodyComp = e.AddComponent<GBox2DBody>()
                .Init()
                .SetType(B2BodyType.b2_dynamicBody)
                .SetMotinLocks(new B2MotionLocks { angularZ = true })
                .SyncToTransform(tr)
                .Build(scene.World.Id)
                .AttacShapes(bid => // -- main body --
                {
                    B2Capsule capsule = B2HelperMethods.MakeCapsule(new(new(-r, 0), new(r, size.Y)));
                    B2ShapeDef capsuleDef = B2Types.b2DefaultShapeDef();
                    capsuleDef.density = 1f;
                    capsuleDef.material.friction = 0.1f;
                    capsuleDef.filter.categoryBits = (ulong)CollisionFlags.PLAYER;
                    capsuleDef.filter.maskBits = (ulong)(CollisionFlags.PLAYER | CollisionFlags.GROUND);
                    //capsuleDef.filter.categoryBits = (ulong)CollisionFlags.PLAYER;
                    B2Shapes.b2CreateCapsuleShape(bid, capsuleDef, capsule);
                }).AttacShapes(bid => // -- circle sensor for ground collision --
                {
                    B2ShapeDef circleSensorDef = B2Types.b2DefaultShapeDef();
                    circleSensorDef.isSensor = true;
                    circleSensorDef.enableSensorEvents = true;
                    circleSensorDef.filter.categoryBits = (ulong)CollisionFlags.FOOT;
                    circleSensorDef.filter.maskBits = (ulong)CollisionFlags.GROUND;
                    var s = B2Shapes.b2CreateCircleShape(bid, circleSensorDef, new(new(0, r * 0.8f), r));
                    var a = B2Bodies.b2Body_GetTransform(B2Shapes.b2Shape_GetBody(s));
                });
            B2Bodies.b2Body_SetUserData(bodyComp.Id, B2UserData.Ref(new PlayerUserData()));
            return e;
        }

        private static Entity CreateHead(GScene scene, B2Vec2 size)
        {
            var e = scene.CreateEntity();
            e.AddComponent<GTransform>().Init(new(0, size.Y, 0), 0f);
            e.AddComponent<GSprite>()
                .SetScale(1.0f / scene.World.PixelPerMetre);
            return e;
        }

        private static Entity CreateWeapon(GScene scene, B2Vec2 size)
        {
            var e = scene.CreateEntity();
            e.AddComponent<GTransform>().Init(new(0, size.Y * 0.6f, 0), 0f);
            e.AddComponent<GSprite>()
                .SetScale(1.0f / scene.World.PixelPerMetre)
                .SetTranslation(new(size.X * 0.25f, 0));
            return e;
        }
    }
}
