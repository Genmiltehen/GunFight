using System.Diagnostics;
using XEngine.Core.Base;
using XEngine.Core.Utils;

namespace XEngine.Core.Common.Health
{
    public class GHealth : GameComponent
    {
        private readonly GameTimer RegenDelay = new(float.MaxValue);
        public float MaxHealth { get; private set; } = 0;
        public float HealthRegen { get; private set; } = 0;
        public float Health { get; private set; } = 0;

        public float Ratio => Health / MaxHealth;
        public bool CanRegenerate => RegenDelay.IsFinished;

        private Action<Entity>? _onDeath = null;

        public GHealth Init(float MaxHealth)
        {
            Health = this.MaxHealth = MaxHealth;
            Owner.Scene.RegisterTimer(RegenDelay);
            return this;
        }

        public GHealth SetRegen(float HealthRegenRate, float HealthRegenDelay)
        {
            HealthRegen = HealthRegenRate;
            RegenDelay.Duration = HealthRegenDelay;
            return this;
        }

        public GHealth SetDeathCallback(Action<Entity> onDeath)
        {
            _onDeath = onDeath;
            return this;
        }

        public void Update(float dt)
        {
            if (CanRegenerate) Health = Math.Clamp(Health + HealthRegen * dt, 0, MaxHealth);
        }

        public void DealDamage(float amount)
        {
            RegenDelay.Start();
            Health -= amount;
            if (Health < 0)
            {
                Health = 0;
                _onDeath?.Invoke(Owner);
            }
        }
    }
}
