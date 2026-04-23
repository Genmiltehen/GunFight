using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEngine.Core.Utils
{
    public static class Box2DMathUtils
    {
        public static float MoveToward(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
                return target;

            return current + Math.Sign(target - current) * maxDelta;
        }
    }
}
