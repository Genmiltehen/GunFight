using Box2D.NET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Input.InputAxis;
using XEngine.Core.Utils.Maths;

namespace XEngine.Core.Utils.Maths
{
    public static class MathUtils
    {
        public const float Epsilon = 1e-6f;

        public static Vector2 FromPolar(float rho, float theta)
        {
            var (s, c) = MathF.SinCos(theta);
            return rho * new Vector2(c, s);
        }
        public static Vector2 Rotate(Vector2 v, float angle)
        {
            var (s, c) = MathF.SinCos(angle);
            return new Vector2(c * v.X + s * v.Y, -s * v.X + c * v.Y);
        }

        public static Vector4 Homogenize(Vector3 v) => new(v.X, v.Y, v.X, 1);
        public static Vector3 Dehomogenize(Vector4 v) => v.Xyz / v.W;

        public static Vector2 FromB2Vec2(B2Vec2 v) => new(v.X, v.Y);
        public static B2Vec2 FromVector2(Vector2 v) => new(v.X, v.Y);

        public static float LimitedStep(float from, float to, float step)
        {
            float diff = to - from;
            return Math.Abs(diff) <= step ? diff : Math.Sign(diff) * step;
        }

        public static Vector2 LimitedStep(Vector2 from, Vector2 to, float step)
        {
            Vector2 diff = to - from;
            return diff.LengthSquared <= step * step ? diff : diff.Normalized() * step;
        }

        public static float MoveToward(float from, float to, float step)
        {
            if (Math.Abs(to - from) <= step)
                return to;

            return from + Math.Sign(to - from) * step;
        }
    }
}
