using Box2D.NET;
using OpenTK.Mathematics;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Player;
using WinFormsUI.Game.Player.Stats;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common.Sprite.NineSlice;
using XEngine.Core.Scenery;
using static XEngine.Core.Box2DCompat.B2Helpers;
using static XEngine.Core.Common.Sprite.SizingPolicy;

namespace WinFormsUI.Game.Scenes
{
    internal static class LevelElementsFabctory
    {
        private readonly static ContactFlags GroundMask = ContactFlags.FOOT | ContactFlags.PLAYER | ContactFlags.SOLID | ContactFlags.PROJECTILE | ContactFlags.ITEM;

        public static Entity CreatePlatform(GScene scene, Vector3 pos, Vector2 size, float rotation = 0)
        {
            var _groundTex = scene.Assets.LoadTexture("Environment\\GroundOrange.png");

            var ground = scene.SpawnEntity();
            ground.Transform.Init(pos, rotation);
            ground.AddComponent<GNineSlice>()
                .SetTexture(_groundTex)
                .SetSizingPolicy(World)
                .SetSize(size)
                .SetBorders(16)
                .SetTranslation(new(0, 0.1f));

            ground.AddComponent<GBox2DBody>()
                .Init(ground.Transform)
                .SetType(B2BodyType.b2_staticBody)
                .Build(scene.World.Id)
                .AttacShapes(bid =>
                {
                    B2Polygon groundBox = B2Geometries.b2MakeBox(size.X / 2, size.Y / 2);
                    B2ShapeDef groundDef = B2Types.b2DefaultShapeDef();
                    groundDef.material.friction = 1f;
                    groundDef.enableSensorEvents = true;
                    groundDef.filter.categoryBits = (ulong)ContactFlags.SOLID;
                    groundDef.filter.maskBits = (ulong)GroundMask;
                    B2Shapes.b2CreatePolygonShape(bid, groundDef, groundBox);
                });
            return ground;
        }

        public static Entity CreateBox(GScene scene, Vector3 pos, Vector2 size, float rotation = 0)
        {
            var _boxTex = scene.Assets.LoadTexture("Environment\\Box.png");

            var box = scene.SpawnEntity();
            box.Transform.Init(pos, rotation);
            box.AddComponent<GNineSlice>()
                .SetTexture(_boxTex)
                .SetSizingPolicy(World)
                .SetSize(size)
                .SetBorders(8);
            box.AddComponent<GBox2DBody>()
                .Init(box.Transform)
                .SetType(B2BodyType.b2_dynamicBody)
                .SetEnableSleep(false)
                .Build(scene.World.Id)
                .AttacShapes(bid =>
                {
                    B2Polygon boxBox = B2Geometries.b2MakeRoundedBox(size.X / 2 - 0.1f, size.Y / 2 - 0.1f, 0.1f);
                    B2ShapeDef boxDef = B2Types.b2DefaultShapeDef();
                    boxDef.material.friction = 1;
                    boxDef.enableSensorEvents = true;
                    boxDef.filter.categoryBits = (ulong)ContactFlags.SOLID;
                    boxDef.filter.maskBits = (ulong)GroundMask;
                    B2Shapes.b2CreatePolygonShape(bid, boxDef, boxBox);
                });
            return box;
        }

        public static Entity CreateLadder(GScene scene, Vector3 pos, Vector2 size, float rotation = 0)
        {
            var _ladderTex = scene.Assets.LoadTexture("Environment\\Ladder.png");

            var ladder = scene.SpawnEntity();
            ladder.Transform.Init(pos, rotation);
            ladder.AddComponent<GNineSlice>()
                .SetTexture(_ladderTex)
                .SetSizingPolicy(World)
                .SetSize(size)
                .SetBorders(5, 2);
            var body = ladder.AddComponent<GBox2DBody>()
                .Init(ladder.Transform)
                .SetType(B2BodyType.b2_staticBody)
                .Build(scene.World.Id)
                .AttacShapes(bid =>
                {
                    B2Polygon ladderBox = B2Geometries.b2MakeBox(size.X / 2, size.Y / 2);
                    B2ShapeDef ladderDef = B2Types.b2DefaultShapeDef();
                    ladderDef.isSensor = true;
                    ladderDef.enableSensorEvents = true;
                    ladderDef.filter.categoryBits = (ulong)ContactFlags.LADDER;
                    ladderDef.filter.maskBits = (ulong)ContactFlags.PLAYER;
                    B2Shapes.b2CreatePolygonShape(bid, ladderDef, ladderBox);
                });

            return ladder;
        }

        public static Entity CreateEffect(GScene scene, Vector3 pos, float ColliderSize, Effect effect)
        {
            var pickupEffect = scene.SpawnEntity();
            pickupEffect.Transform.Init(pos, 0);
            pickupEffect.AddComponent<GHeldEffect>().Init(effect);
            var bodyComp = pickupEffect.AddComponent<GBox2DBody>()
                .Init(pickupEffect.Transform)
                .SetType(B2BodyType.b2_staticBody)
                .Build(scene.World.Id)
                .EnableCollisionCallback()
                .AttacShapes(bid =>
                {
                    B2ShapeDef circleSensorDef = B2Types.b2DefaultShapeDef();
                    circleSensorDef.isSensor = true;
                    circleSensorDef.enableSensorEvents = true;
                    circleSensorDef.filter.categoryBits = (ulong)ContactFlags.EFFECT;
                    circleSensorDef.filter.maskBits = (ulong)ContactFlags.PLAYER;
                    B2Shapes.b2CreateCircleShape(bid, circleSensorDef, new(new(0, 0), ColliderSize * 0.5f));
                });
            bodyComp.OnCollisionEnter = EffectTouchCB;

            return pickupEffect;
        }

        private static void EffectTouchCB(ContactWrapper ev)
        {
            if (ev.IsSensor && CheckFlag(ev.ShapeIdA, (ulong)ContactFlags.EFFECT))
            {
                Effect effect = ev.EntityA!.Get<GHeldEffect>()!.Effect;
                ev.GBodyB!.Owner.Get<GPlayer>()?.Effects.Add(effect);
                ev.EntityA?.MarkDelete();
            }
        }
    }
}
