using Box2D.NET;
using OpenTK.Mathematics;
using System.Diagnostics;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Config;
using WinFormsUI.Game.Player;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;
using XEngine.Core.Box2DCompat;
using XEngine.Core.Common.LifeTime;
using XEngine.Core.Common.Trace;
using static Box2D.NET.B2MathFunction;
using static Box2D.NET.B2Bodies;
using static Box2D.NET.B2Shapes;
using XEngine.Core.Common.Health;

namespace WinFormsUI.Game.Combat.Projectiles
{
    public class ProjectileFactory
    {
        private readonly ConfigLoader<ProjectileConfig> configLoader;
        private static readonly Lazy<ProjectileFactory> _instance = new(() => new ProjectileFactory());
        public static ProjectileFactory Instance => _instance.Value;
        private ProjectileFactory()
        {
            configLoader = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Config", "projectiles.json"));
        }

        private static Action<B2BodyId> CreateCollider(ProjectileConfig config)
        {
            return bid =>
            {
                B2ShapeDef segSensorDef = B2Types.b2DefaultShapeDef();
                segSensorDef.isSensor = true;
                segSensorDef.enableSensorEvents = true;
                segSensorDef.filter.categoryBits = (ulong)CollisionFlags.PROJECTILE;
                segSensorDef.filter.maskBits = (ulong)(CollisionFlags.GROUND | CollisionFlags.PLAYER);
                B2Segment seg = new(new(-config.Size * 0.5f, 0), new(config.Size * 0.5f, 0));
                b2CreateSegmentShape(bid, segSensorDef, seg);
            };
        }

        public Entity? CreateProjectile(string projectileId, string source, GScene scene, Vector2 pos, Vector2 vel)
        {
            if (!configLoader.TryGetConfig(projectileId, out var config))
            {
                Debug.WriteLine($"[Warn]: Could not spawn projectile, {projectileId} is undefined");
                return null;
            }

            var projectile = scene.SpawnEntity();
            projectile.Transform.Init(new(pos.X, pos.Y, 0), MathF.Atan2(vel.Y, vel.X));

            projectile.AddComponent<GSprite>()
                .SetTexture(scene.Assets.LoadTexture(config.TexturePath), false)
                .SetSize(new(config.Size * scene.World.PixelPerMetre, 1));
            var bulletTracer = scene.SpawnEntity();
            bulletTracer.AddComponent<GTrace>().Init(new(1, 0.5f, 0), 10).IsAggressive = true;
            bulletTracer.Transform.SetParent(projectile.Transform);

            projectile.AddComponent<GProjectile>().Init(config).SetSource(source);
            projectile.AddComponent<GLifeTime>().Init(config.MaxLifetime);
            var bodyComp = projectile.AddComponent<GBox2DBody>().Init()
                .SetType(B2BodyType.b2_kinematicBody)
                .SetMotinLocks(new B2MotionLocks() { angularZ = true })
                .SyncToTransform(projectile.Transform)
                .Build(scene.World.Id)
                .EnableCollisionCallbacks()
                .AttacShapes(CreateCollider(config));

            b2Body_SetLinearVelocity(bodyComp.Id, new(vel.X, vel.Y));
            bodyComp.OnCollisionEnter = BulletCollisionCallback;

            return projectile;
        }

        private void BulletCollisionCallback(ContactWrapper ev)
        {
            if (ev.IsSensor && b2Shape_GetFilter(ev.ShapeIdA).categoryBits == (ulong)CollisionFlags.PROJECTILE)
            {
                var projectile = ev.GBodyA!.Owner.Get<GProjectile>()!;
                var target = ev.EntityB;
                if (target?.TryGet<GPlayer>(out var player) == true)
                {
                    if (player.Name != projectile.Source)
                    {
                        float damage = MathF.Max(0, projectile.Damage - player.Stats.Armor);
                        target.Get<GHealth>()?.DealDamage(damage);
                    }
                    else return;
                }
                projectile.Owner.MarkDelete();
            }
        }
    }
}
