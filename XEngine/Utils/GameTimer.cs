using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEngine.Core.Utils
{
    public class GameTimer
    {
        public bool IsRunning { get; internal set; }
        public float Duration;
        private float elapsed;

        public GameTimer(float duration)
        {
            this.Duration = duration;
            Reset();
        }

        internal void Tick(float deltaTime)
        {
            if (!IsRunning) return;
            elapsed += deltaTime;
            if (elapsed >= Duration)
            {
                elapsed = Duration;
                IsRunning = false;
                OnComplete?.Invoke();
            }
        }

        public GameTimer Start()
        {
            elapsed = 0;
            IsRunning = true;
            return this;
        }

        public GameTimer Reset()
        {
            elapsed = 0;
            IsRunning = false;
            return this;
        }

        public bool IsFinished => elapsed >= Duration;
        public float Progress => elapsed / Duration;

        public event Action? OnComplete;
    }
}
