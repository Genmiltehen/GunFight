using XEngine.Core.Common;

namespace WinFormsUI.Game.Player.Stats
{
    public abstract class Effect : IPlayerStats
    {
        protected IPlayerStats? _stats;
        public readonly GameTimer Timer = new(float.MaxValue);

        public virtual float TopSpeed => _stats?.TopSpeed ?? 0f;
        public virtual float Acceleration => _stats?.Acceleration ?? 0f;
        public virtual float JumpPower => _stats?.JumpPower ?? 0f;
        public virtual float Armor => _stats?.Armor ?? 0f;

        public Effect SetBase(IPlayerStats stats)
        {
            _stats = stats;
            return this;
        }

        public Effect Duration(float duration)
        {
            Timer.Duration = duration;
            return this;
        }
    }
}
