using Box2D.NET;
using OpenTK.Mathematics;
using System.Diagnostics;
using WinFormsUI.Game.Box2D;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite.NineSlice;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes
{
    internal static class LevelElementsFabctory
    {
        private readonly static CollisionFlags GroundMask = CollisionFlags.FOOT | CollisionFlags.PLAYER | CollisionFlags.GROUND | CollisionFlags.PROJECTILE;
        
        public static Entity CreatePlatform(GScene scene, Vector3 pos, B2Vec2 size, float rotation = 0)
        {
            var _groundTex = scene.Assets.LoadTexture("Environment\\GroundOrange.png");

            var ground = scene.SpawnEntity();
            ground.Transform.Init(new(pos.X, pos.Y, 0), rotation);
            ground.AddComponent<GNineSlice>()
                .SetTexture(_groundTex, false)
                .SetSize(new Vector2(size.X, size.Y) * scene.World.PixelPerMetre)
                .SetBorders(16)
                .SetTranslation(new(0, 0.1f));

            ground.AddComponent<GBox2DBody>()
                .Init()
                .SetType(B2BodyType.b2_staticBody)
                .SyncToTransform(ground.Transform)
                .Build(scene.World.Id)
                .AttacShapes(bid =>
                {
                    B2Polygon groundBox = B2Geometries.b2MakeBox(size.X / 2, size.Y / 2);
                    B2ShapeDef groundDef = B2Types.b2DefaultShapeDef();
                    groundDef.material.friction = 1f;
                    groundDef.enableSensorEvents = true;
                    groundDef.filter.categoryBits = (ulong)CollisionFlags.GROUND;
                    groundDef.filter.maskBits = (ulong)GroundMask;
                    B2Shapes.b2CreatePolygonShape(bid, groundDef, groundBox);
                });
            return ground;
        }

        public static Entity CreateBox(GScene scene, Vector3 pos, B2Vec2 size, float rotation = 0)
        {
            var _boxTex = scene.Assets.LoadTexture("Environment\\Box.png");

            var box = scene.SpawnEntity();
            box.Transform.Init(new(pos.X, pos.Y, 0), rotation);
            box.AddComponent<GNineSlice>()
                .SetBorders(8)
                .SetTexture(_boxTex, false)
                .SetSize(new Vector2(size.X, size.Y) * scene.World.PixelPerMetre);
            box.AddComponent<GBox2DBody>()
                .Init()
                .SetType(B2BodyType.b2_dynamicBody)
                .SyncToTransform(box.Transform)
                .SetEnableSleep(false)
                .Build(scene.World.Id)
                .AttacShapes(bid =>
                {
                    B2Polygon boxBox = B2Geometries.b2MakeRoundedBox(size.X / 2 - 0.1f, size.Y / 2 - 0.1f, 0.1f);
                    B2ShapeDef boxDef = B2Types.b2DefaultShapeDef();
                    boxDef.material.friction = 1;
                    boxDef.enableSensorEvents = true;
                    boxDef.filter.categoryBits = (ulong)CollisionFlags.GROUND;
                    boxDef.filter.maskBits = (ulong)GroundMask;
                    B2Shapes.b2CreatePolygonShape(bid, boxDef, boxBox);
                });
            return box;
        }
    }
}
