using Box2D.NET;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Combat;
using WinFormsUI.Game.Config;
using WinFormsUI.Game.Player.Components;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Common.Health;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Player
{
    public class PlayerFactory
    {
        private const CollisionFlags PlayerMask = CollisionFlags.GROUND | CollisionFlags.PROJECTILE;
        private readonly static float _r = 0.5f * GPlayerModel.PLAYER_SIZE.X;
        private readonly ConfigLoader<PlayerConfig> configLoader;

        private static readonly Lazy<PlayerFactory> _instance = new(() => new PlayerFactory());
        public static PlayerFactory Instance => _instance.Value;
        private PlayerFactory()
        {
            configLoader = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Config", "players.json"));
        }

        public GPlayer CreatePlayer(GScene scene, B2Vec2 pos, string name, string playerConfigId)
        {
            if (!configLoader.TryGetConfig(playerConfigId, out var config)) throw new ArgumentException("player Id not found");

            var playerEntity = CreateBody(scene, pos);
            playerEntity.AddComponent<GHealth>()
                .Init(config.MaxHealth)
                .SetRegen(config.HealthRegenRate, config.HealthRegenDelay)
                .SetDeathCallback(e => e.MarkDelete(true));
            return playerEntity.AddComponent<GPlayer>().Init(scene, config).SetName(name);
        }

        private static void CreateCollider(B2BodyId bid)
        {
            B2Capsule capsule = B2Helpers.MakeCapsule(new(new(-_r, 0), new(_r, GPlayerModel.PLAYER_SIZE.Y)));
            B2ShapeDef capsuleDef = B2Types.b2DefaultShapeDef();
            capsuleDef.density = 1f;
            capsuleDef.material.friction = 0.1f;
            capsuleDef.enableSensorEvents = true;
            capsuleDef.filter.categoryBits = (ulong)CollisionFlags.PLAYER;
            capsuleDef.filter.maskBits = (ulong)PlayerMask;
            B2Shapes.b2CreateCapsuleShape(bid, capsuleDef, capsule);
        }

        private static void CreateFoot(B2BodyId bid)
        {
            B2ShapeDef circleSensorDef = B2Types.b2DefaultShapeDef();
            circleSensorDef.isSensor = true;
            circleSensorDef.enableSensorEvents = true;
            circleSensorDef.filter.categoryBits = (ulong)CollisionFlags.FOOT;
            circleSensorDef.filter.maskBits = (ulong)CollisionFlags.GROUND;
            B2Shapes.b2CreateCircleShape(bid, circleSensorDef, new(new(0, _r * 0.9f), _r * 0.95f));
        }

        private static Entity CreateBody(GScene scene, B2Vec2 pos)
        {
            var e = scene.SpawnEntity();
            e.Transform.Init(new(pos.X, pos.Y, 0), 0f);
            e.AddComponent<GSprite>().SetTranslation(new(0, 0.75f));

            var bodyComp = e.AddComponent<GBox2DBody>()
                .Init()
                .SetType(B2BodyType.b2_dynamicBody)
                .SetMotinLocks(new B2MotionLocks { angularZ = true })
                .SyncToTransform(e.Transform)
                .Build(scene.World.Id)
                .EnableCollisionCallbacks()
                .AttacShapes(CreateCollider)
                .AttacShapes(CreateFoot);

            bodyComp.OnCollisionEnter = PlayerCollisionEnter;
            bodyComp.OnCollisionExit = PlayerCollisionExit;
            return e;
        }

        private static void PlayerCollisionEnter(ContactWrapper ev)
        {
            if (ev.IsSensor && B2Shapes.b2Shape_GetFilter(ev.ShapeIdA).categoryBits == (ulong)CollisionFlags.FOOT)
                ev.GBodyA!.Owner.Get<GPlayerMovement>()!.GroundContacts++;
        }

        private static void PlayerCollisionExit(ContactWrapper ev)
        {
            if (ev.IsSensor && B2Shapes.b2Shape_GetFilter(ev.ShapeIdA).categoryBits == (ulong)CollisionFlags.FOOT)
                ev.GBodyA!.Owner.Get<GPlayerMovement>()!.GroundContacts--;
        }
    }
}
