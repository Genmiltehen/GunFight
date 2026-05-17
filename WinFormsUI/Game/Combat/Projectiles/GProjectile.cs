using XEngine.Core.Base;

namespace WinFormsUI.Game.Combat.Projectiles
{
    public class GProjectile : GameComponent
    {
        public float Damage { get; set; } = 0;

        public GProjectile Init(float damage)
        {
            Damage = damage;
            return this;
        }
    }
}
