using Box2D.NET;
using OpenTK.Mathematics;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Combat.Weapons;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common.LifeTime;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Common.Transform;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Drop
{
    public struct DropBuilder
    {
        private static readonly B2Vec2 size = new(0.5f, 0.25f);

        private readonly WeaponItem item;
        private float expirationTime = float.MaxValue;
        private B2Vec2 linVel = new(0, 0);
        private float angVel = 0;
        private bool canPickup = true;

        private DropBuilder(WeaponItem Item)
        {
            item = Item;
        }

        public static DropBuilder Init(WeaponItem item)
        {
            return new(item);
        }

        public DropBuilder SetExpirationTime(float time)
        {
            expirationTime = time;
            return this;
        }

        public DropBuilder CanPickup(bool value)
        {
            canPickup = value;
            return this;
        }

        public DropBuilder SetVelocity(Vector2 linear, float angular = 0)
        {
            linVel = new(linear.X, linear.Y);
            angVel = angular;
            return this;
        }

        public readonly Entity Spawn(GScene scene, Vector3 pos) => Spawn(scene, new TransformValues() { Position = pos });
        public readonly Entity Spawn(GScene scene, TransformValues values)
        {
            var e = scene.SpawnEntity();
            bool flip = MathF.Abs(values.Rotation) > MathF.PI / 2;
            values.Rotation = flip ? MathF.PI - values.Rotation : values.Rotation;
            e.Transform.Init(values);

            e.AddComponent<GDrop>().Init(item).SetPickupFlag(canPickup);

            e.AddComponent<GLifeTime>().Init(expirationTime).IsBlinking = expirationTime != float.MaxValue;

            e.AddComponent<GSprite>()
                .SetSizingPolicy(SizingPolicy.Source)
                .SetSize(item.TexSize)
                .SetTexture(item.SavedTexture).FlipX = flip;

            var bodyComp = e.AddComponent<GBox2DBody>()
                .Init(e.Transform)
                .SetType(B2BodyType.b2_dynamicBody)
                .Build(scene.World.Id)
                .EnableCollisionCallback()
                .AttacShapes(CreateCollider)
                .AttacShapes(CreateItemHitbox);

            bodyComp.LinearVelocity = linVel;
            bodyComp.AngularVelocity = angVel;

            return e;
        }

        private static void CreateCollider(B2BodyId bid)
        {
            B2Capsule capsule = B2Helpers.MakeCapsule(new(size * -0.5f, size * 0.5f));
            B2ShapeDef capsuleDef = B2Types.b2DefaultShapeDef();
            capsuleDef.density = 1f;
            capsuleDef.material.friction = 1f;
            capsuleDef.filter.categoryBits = (ulong)ContactFlags.ITEM;
            capsuleDef.filter.maskBits = (ulong)ContactFlags.SOLID;
            B2Shapes.b2CreateCapsuleShape(bid, capsuleDef, capsule);
        }

        private static void CreateItemHitbox(B2BodyId bid)
        {
            B2Vec2 size = new(0.5f, 0.25f);
            B2Capsule capsule = B2Helpers.MakeCapsule(new(size * -0.5f, size * 0.5f));
            B2ShapeDef capsuleDef = B2Types.b2DefaultShapeDef();
            capsuleDef.isSensor = true;
            capsuleDef.enableSensorEvents = true;
            capsuleDef.filter.categoryBits = (ulong)ContactFlags.ITEM_HITBOX;
            capsuleDef.filter.maskBits = (ulong)ContactFlags.PLAYER;
            B2Shapes.b2CreateCapsuleShape(bid, capsuleDef, capsule);
        }
    }
}
