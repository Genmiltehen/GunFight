using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;

namespace WinFormsUI.Game.Player.Stats
{
    public abstract class PlayerEffect : IPlayerStats
    {
        protected IPlayerStats? _stats;

        public virtual float TopSpeed => _stats?.TopSpeed ?? 0f;
        public virtual float Acceleration => _stats?.Acceleration ?? 0f;
        public virtual float JumpPower => _stats?.JumpPower ?? 0f;
        public virtual float Armor => _stats?.Armor ?? 0f;

        protected PlayerEffect() { }
        public PlayerEffect SetBase(IPlayerStats stats)
        {
            _stats = stats;
            return this;
        }
    }
}
