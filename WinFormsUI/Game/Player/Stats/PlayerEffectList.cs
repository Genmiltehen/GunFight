using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI.Game.Player.Stats
{
    public class PlayerEffectList(IPlayerStats sourceStats)
    {
        private readonly List<PlayerEffect> _effects = [];
        private IPlayerStats _sourceStats = sourceStats;
        private IPlayerStats _modifiedStats = sourceStats;
        private bool _isDirty = true;

        public PlayerEffectList SetSource(IPlayerStats sourceStats)
        {
            _sourceStats = sourceStats;
            _isDirty = true;
            return this;
        }

        public PlayerEffectList Add(PlayerEffect effect)
        {
            _effects.Add(effect);
            _isDirty = true;
            return this;
        }

        public PlayerEffectList Remove(PlayerEffect effect)
        {
            _effects.Remove(effect);
            _isDirty = true;
            return this;
        }

        public IPlayerStats GetStats()
        {
            if (_isDirty) Recalculate();
            return _modifiedStats;
        }

        private void Recalculate()
        {
            _isDirty = false;
            IPlayerStats res = _sourceStats;
            foreach (PlayerEffect effect in _effects) res = effect.SetBase(res);
            _modifiedStats = res;
        }
    }
}
