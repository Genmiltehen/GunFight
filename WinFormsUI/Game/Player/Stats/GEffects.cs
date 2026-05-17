using XEngine.Core.Base;

namespace WinFormsUI.Game.Player.Stats
{
    public class GEffects : GameComponent
    {
        private readonly List<Effect> _effects = [];
        private IPlayerStats _sourceStats = null!;
        private IPlayerStats _cacheStats = null!;
        private bool _isDirty = true;

        public GEffects SetSource(IPlayerStats sourceStats)
        {
            _sourceStats = sourceStats;
            _isDirty = true;
            return this;
        }

        public GEffects Add<T>(T effect) where T : Effect
        {
            if (TryGetEffectOfType<T>(out var _oldEffect)) _oldEffect.Timer.Duration += effect.Timer.Duration;
            else
            {
                Owner.Scene.RegisterTimer(effect.Timer);
                effect.Timer.Start();
                _effects.Add(effect);
                _isDirty = true;
            }
            return this;
        }

        private bool TryGetEffectOfType<T>(out T effect) where T : Effect
        {
            effect = null!;
            var effectsOftype = _effects.OfType<T>();
            if (effectsOftype.Any())
            {
                effect = effectsOftype.First();
                return true;
            }
            return false;
        }

        public GEffects Remove(Effect effect)
        {
            Owner.Scene.UnregisterTimer(effect.Timer);
            _effects.Remove(effect);
            _isDirty = true;
            return this;
        }

        

        public IPlayerStats GetStats()
        {
            if (_isDirty) Recalculate();
            return _cacheStats;
        }

        public void WearoffEffects()
        {
            if (!_effects.Any(eff => eff.Timer.IsFinished)) return;
            var effects = _effects.Where(eff => eff.Timer.IsFinished).ToList();
            foreach (var effect in effects) Remove(effect);
        }

        private void Recalculate()
        {
            _isDirty = false;
            IPlayerStats res = _sourceStats;
            foreach (Effect effect in _effects) res = effect.SetBase(res);
            _cacheStats = res;
        }
    }
}
