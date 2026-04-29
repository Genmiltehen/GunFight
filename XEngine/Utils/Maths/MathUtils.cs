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

        public static Vector4 Homogenize(Vector3 v) => new(v.X, v.Y, v.X, 1);
        public static Vector3 Dehomogenize(Vector4 v) => v.Xyz / v.W;

        public static float Cross2D(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static Vector2 LeftPerp(Vector2 v)
        {
            return new Vector2(v.Y, -v.X);
        }

        public static Vector2 RightPerp(Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        public static Vector2 PerpTowards(Vector2 v, Vector2 normal)
        {
            Vector2 res = new(v.Y, -v.X);
            if (Vector2.Dot(res, normal) < 0) res = -res;
            return res;
        }

        public static Vector2 FlipTowards(Vector2 v, Vector2 direction)
        {
            return Vector2.Dot(direction, v) >= 0 ? v : -v;
        }

        public static void CalcAxes(float rotation, out Vector2 axis1, out Vector2 axis2)
        {
            var (sin, cos) = MathF.SinCos(rotation);
            axis1 = new Vector2(cos, sin);
            axis2 = new Vector2(-sin, cos);
        }

        public static bool Box2Intersect(Box2 a, Box2 b)
        {
            return a.Min.X <= b.Max.X && a.Max.X >= b.Min.X &&
                   a.Min.Y <= b.Max.Y && a.Max.Y >= b.Min.Y;
        }

        public static Vector2 LineProjection(Vector2 p, Segment line)
        {
            return line.start + Vector2.Dot(p - line.start, line.Direction) * line.Direction;
        }

        public static Vector2 LineProjectionClipped(Vector2 p, Segment seg)
        {
            return seg.ClampLerp(Vector2.Dot(p - seg.start, seg.Direction) / seg.Length);
        }

        public static float MoveToward(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
                return target;

            return current + Math.Sign(target - current) * maxDelta;
        }
    }
}
