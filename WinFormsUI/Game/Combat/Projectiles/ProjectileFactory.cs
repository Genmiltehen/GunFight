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

using static Box2D.NET.B2MathFunction;
using static Box2D.NET.B2Bodies;
using static Box2D.NET.B2Shapes;

namespace WinFormsUI.Game.Combat.Projectiles
{
    public class ProjectileFactory(ConfigLoader<ProjectileConfig> configCenter)
    {
        public Entity? CreateProjectile(string projectileId, string source, GScene scene, Vector2 pos, float angle)
        {
            if (!configCenter.TryGetConfig(projectileId, out var config))
            {
                Debug.WriteLine($"[Warn]: Could not spawn projectile, {projectileId} is undefined");
                return null;
            }

            var e = scene.CreateEntity();
            e.Transform.Init(new(pos.X, pos.Y, 0), angle);

            e.AddComponent<GSprite>()
                .SetTexture(scene.Assets.LoadTexture(config.TexturePath), false)
                .SetSize(new(config.Size * scene.World.PixelPerMetre, 1));

            e.AddComponent<GProjectile>().Init(config, scene).SetSource(source);

            var bodyComp = e.AddComponent<GBox2DBody>().Init()
                .SetType(B2BodyType.b2_kinematicBody)
                .SyncToTransform(e.Transform)
                .Build(scene.World.Id)
                .EnableCollisionCallbacks()
                .AttacShapes(bid =>
                {
                    B2ShapeDef segSensorDef = B2Types.b2DefaultShapeDef();
                    segSensorDef.isSensor = true;
                    segSensorDef.enableSensorEvents = true;
                    segSensorDef.filter.categoryBits = (ulong)CollisionFlags.PROJECTILE;
                    segSensorDef.filter.maskBits = (ulong)(CollisionFlags.GROUND | CollisionFlags.PLAYER);
                    B2Segment seg = new(new(-config.Size * 0.5f, 0), new(config.Size * 0.5f, 0));
                    b2CreateSegmentShape(bid, segSensorDef, seg);
                });

            B2Vec2 vel = b2RotateVector(b2MakeRot(angle), new(config.InitialVelocity, 0));
            b2Body_SetLinearVelocity(bodyComp.Id, vel);
            bodyComp.OnCollisionEnter = (ev) =>
            {
                if (ev.IsSensor && B2Shapes.b2Shape_GetFilter(ev.ShapeIdA).categoryBits == (ulong)CollisionFlags.PROJECTILE)
                {
                    var projectile = ev.GBodyA!.Owner.Get<GProjectile>()!;
                    var target = ev.EntityB;
                    if (target?.TryGet<GPlayer>(out var player) == true)
                    {
                        if (player.Name != projectile.Source)
                        {
                            Debug.WriteLine($"Player {player.Name} hurt by {projectile.Damage}");
                            // Deal Damage
                        } else return;
                    }
                    projectile.Owner.MarkDelete();
                }
            };
            return e;
        }
    }
}
