using XEngine.Core.Base;

namespace WinFormsUI.Game.Combat.Projectiles
{
    public class GProjectile : GameComponent
    {
        public string Source { get; set; } = null!;
        public float Damage { get; set; } = 0;

        public GProjectile Init(ProjectileConfig config)
        {
            Damage = config.Damage;
            return this;
        }
    }
}
