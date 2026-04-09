using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEngine.Core.Input.InputAxis
{
    public class Axis(AxisSettings settings)
    {
        public string Positive = settings.Positive;
        public string Negative = settings.Negative;
        public float Gravity = settings.Gravity;
        public float Sensitivity = settings.Sensitivity;
        public float Value { get; private set; } = 0;

        public void Update(InputService input, float dt)
        {
            float target = 0;
            float current = Value;

            if (input.IsActionActive(Positive)) target += 1;
            if (input.IsActionActive(Negative)) target -= 1;

            if (target != 0) current = MoveTowards(current, target, Sensitivity * dt);
            else current = MoveTowards(current, 0, Gravity * dt);

            Value = current;
        }

        private static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta) return target;
            return current + Math.Sign(target - current) * maxDelta;
        }
    }
}
