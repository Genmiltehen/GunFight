using Box2D.NET;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Config;
using WinFormsUI.Game.Player.Stats;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Player
{
    public class PlayerFactory(ConfigLoader<PlayerConfig> configLoader)
    {
        private readonly static CollisionFlags PlayerMask = CollisionFlags.GROUND | CollisionFlags.PROJECTILE;

        public GPlayer CreatePlayer(GScene scene, B2Vec2 pos, string name, string playerConfigId)
        {
            if (!configLoader.TryGetConfig(playerConfigId, out var config)) throw new ArgumentException("player Id not found");

            var body = CreateBody(scene, pos);
            CreateHead(scene).Get<GTransform>()!.SetParent(body.Transform);
            CreateWeapon(scene).Get<GTransform>()!.SetParent(body.Transform);

            var player = body.AddComponent<GPlayer>().Init(scene, config).SetName(name);
            player.CharacterSpriteName = config.TextureName;
            PlayerHelper.SetFacing(player, new(1, 0), true);

            return player;
        }

        private static Entity CreateBody(GScene scene, B2Vec2 pos)
        {
            var e = scene.CreateEntity();
            e.Transform.Init(new(pos.X, pos.Y, 0), 0f);
            e.AddComponent<GSprite>().SetTranslation(new(0, 0.75f));

            float r = 0.5f * PlayerHelper.PLAYER_SIZE.X;
            var bodyComp = e.AddComponent<GBox2DBody>()
                .Init()
                .SetType(B2BodyType.b2_dynamicBody)
                .SetMotinLocks(new B2MotionLocks { angularZ = true })
                .SyncToTransform(e.Transform)
                .Build(scene.World.Id)
                .EnableCollisionCallbacks()
                .AttacShapes(bid => // -- main body --
                {
                    B2Capsule capsule = B2Helpers.MakeCapsule(new(new(-r, 0), new(r, PlayerHelper.PLAYER_SIZE.Y)));
                    B2ShapeDef capsuleDef = B2Types.b2DefaultShapeDef();
                    capsuleDef.density = 1f;
                    capsuleDef.material.friction = 0.1f;
                    capsuleDef.enableSensorEvents = true;
                    capsuleDef.filter.categoryBits = (ulong)CollisionFlags.PLAYER;
                    capsuleDef.filter.maskBits = (ulong)PlayerMask;
                    B2Shapes.b2CreateCapsuleShape(bid, capsuleDef, capsule);
                }).AttacShapes(bid => // -- circle sensor for ground collision --
                {
                    B2ShapeDef circleSensorDef = B2Types.b2DefaultShapeDef();
                    circleSensorDef.isSensor = true;
                    circleSensorDef.enableSensorEvents = true;
                    circleSensorDef.filter.categoryBits = (ulong)CollisionFlags.FOOT;
                    circleSensorDef.filter.maskBits = (ulong)CollisionFlags.GROUND;
                    B2Shapes.b2CreateCircleShape(bid, circleSensorDef, new(new(0, r * 0.9f), r * 0.95f));
                });
            bodyComp.OnCollisionEnter = (ev) =>
            {
                if (ev.IsSensor && B2Shapes.b2Shape_GetFilter(ev.ShapeIdA).categoryBits == (ulong)CollisionFlags.FOOT)
                    ev.GBodyA!.Owner.Get<GPlayer>()!.GroundContacts++;
            };
            bodyComp.OnCollisionExit = (ev) =>
            {
                if (ev.IsSensor && B2Shapes.b2Shape_GetFilter(ev.ShapeIdA).categoryBits == (ulong)CollisionFlags.FOOT)
                    ev.GBodyA!.Owner.Get<GPlayer>()!.GroundContacts--;
            };
            return e;
        }

        private static Entity CreateHead(GScene scene)
        {
            var e = scene.CreateEntity();
            e.AddComponent<GSprite>();
            return e;
        }

        private static Entity CreateWeapon(GScene scene)
        {
            var e = scene.CreateEntity();
            e.AddComponent<GSprite>();
            return e;
        }
    }
}
