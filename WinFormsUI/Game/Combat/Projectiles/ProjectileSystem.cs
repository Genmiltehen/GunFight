using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Combat.Projectiles
{
    internal class ProjectileSystem : IGameSystem
    {
        public int Priority => 200;

        public bool IsEnabled { get; set; } = true;

        public void Update(GScene _scene, float _dt)
        {
            foreach (var (e, body) in _scene.Query<GBox2DBody>((e, _) => e.Has<GProjectile>()))
            {
                var vel = body.LinearVelocity;
                e.Transform.Rotation = MathF.Atan2(vel.Y, vel.X);
            }
        }
    }
}
