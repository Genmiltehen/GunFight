using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Utils;

namespace XEngine.Core.Common.LifeTime
{
    public class GLifeTime : GameComponent
    {
        public readonly GameTimer LifeTimer = new(float.MaxValue);

        public GLifeTime Init(float lifeTime)
        {
            LifeTimer.Duration = lifeTime;
            Owner.Scene.RegisterTimer(LifeTimer);
            LifeTimer.Start();
            return this;
        }
    }
}
