using OpenTK.Mathematics;
using System.Diagnostics;
using XEngine.Core.Input.InputAxis;

namespace XEngine.Core.Utils
{
    public static class MathUtils
    {
        public const float Epsilon = 1e-6f;

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

        public static void ClosestPointsBetweenSegments(Segment seg1, Segment seg2, out Vector2 closestPoint1, out Vector2 closestPoint2)
        {
            Vector2 r = seg1.start - seg2.start;
            Vector2 d1 = seg1.Vector;
            Vector2 d2 = seg2.Vector;

            float d1sq = Vector2.Dot(d1, d1);
            float d2sq = Vector2.Dot(d2, d2);
            float rd1 = Vector2.Dot(d1, r);
            float rd2 = Vector2.Dot(d2, r);
            float d1d2 = Vector2.Dot(d1, d2);
            float denom = d1sq * d2sq - d1d2 * d1d2;

            float s, t;

            if (d1sq > Epsilon && d2sq > Epsilon)
            {
                s = denom <= Epsilon ? 0.5f : Math.Clamp((d1d2 * rd2 - rd1 * d2sq) / denom, 0f, 1f);
                t = (d1d2 * s + rd2) / d2sq;

                if (t < 0)
                {
                    t = 0;
                    s = Math.Clamp(-rd1 / d1sq, 0f, 1f);
                }
                else if (t > 1)
                {
                    t = 1;
                    s = Math.Clamp((d1d2 - rd1) / d1sq, 0f, 1f);
                }
            }
            else if (d1sq <= Epsilon)
            {
                s = 0f;
                t = Math.Clamp(rd2 / d2sq, 0f, 1f);
            }
            else if (d2sq <= Epsilon)
            {
                s = Math.Clamp(-rd1 / d1sq, 0f, 1f);
                t = 0f;
            }
            else
            {
                s = 0f;
                t = 0f;
            }

            closestPoint1 = seg1.Lerp(s);
            closestPoint2 = seg2.Lerp(t);
        }

        public static Vector2 LineProjection(Vector2 p, Segment line)
        {
            return line.start + Vector2.Dot(p - line.start, line.Direction) * line.Direction;
        }

        public static Vector2 LineProjectionClipped(Vector2 p, Segment seg)
        {
            return seg.ClampLerp(Vector2.Dot(p - seg.start, seg.Direction) / seg.Length);
        }

        public static void PolygonProjectionBounds(Vector2 axis, Vector2[] points, out float min, out float max)
        {
            max = min = Vector2.Dot(axis, points[0]);

            for (int i = 1; i < points.Length; i++)
            {
                float projection = Vector2.Dot(axis, points[i]);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }
        }

        public static Vector2[] GetPolygonNormals(Vector2[] points)
        {
            Vector2[] axes = new Vector2[points.Length];
            Vector2 prev = points[^1];
            for (int i = 0; i < points.Length; i++)
            {
                axes[i] = Vector2.Normalize(LeftPerp(points[i] - prev));
                prev = points[i];
            }
            return axes;
        }

        public static Vector2 CenterOfMass(Vector2[] points)
        {
            Vector2 center = Vector2.Zero;
            for (int i = 0; i < points.Length; i++) center += points[i];
            return center / points.Length;
        }
    }
}
