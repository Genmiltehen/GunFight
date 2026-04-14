using OpenTK.Mathematics;
using System.Linq;
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

        public static bool Box2Intersect(Box2 a, Box2 b)
        {
            return a.Min.X <= b.Max.X && a.Max.X >= b.Min.X &&
                   a.Min.Y <= b.Max.Y && a.Max.Y >= b.Min.Y;
        }

        public static void ClosestPointsBetweenSegments(
            Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2,
            out Vector2 closestPoint1, out Vector2 closestPoint2)
        {
            Vector2 r = p1 - p2;

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

            closestPoint1 = p1 + d1 * s;
            closestPoint2 = p2 + d2 * t;
        }

        public static Vector2 DirFromLineToPoint(Vector2 p, Vector2 origin, Vector2 dir)
        {
            Vector2 p_prime = p - origin;
            float h = Vector2.Dot(p_prime, dir);
            return p_prime - dir * h;
        }

        public static void PolygonBoundsAlongAxis(Vector2 axis, Vector2[] points, out float min, out float max)
        {
            max = min = Vector2.Dot(axis, points[0]);

            for (int i = 1; i < points.Length; i++)
            {
                float projection = Vector2.Dot(axis, points[i]);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }
        }

        public static Vector2[] GetPolygonAxes(Vector2[] points)
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

        public static bool TestSAT(Vector2[] pointsA, Vector2[] pointsB, out Vector2 normal)
        {
            normal = Vector2.Zero;
            float minOverlap = float.MaxValue;
            IEnumerable<Vector2> allAxes = GetPolygonAxes(pointsA).Concat(GetPolygonAxes(pointsB));

            foreach (Vector2 axis in allAxes)
            {
                PolygonBoundsAlongAxis(axis, pointsA, out float minA, out float maxA);
                PolygonBoundsAlongAxis(axis, pointsB, out float minB, out float maxB);
                if (minA > maxB || minB > maxA) return false;

                float overlap = Math.Min(maxA, maxB) - Math.Max(minA, minB);
                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    normal = axis;
                }
            }

            Vector2 centerA = CenterOfMass(pointsA);
            Vector2 centerB = CenterOfMass(pointsB);
            if (Vector2.Dot(normal, centerB - centerA) < 0) normal = -normal;
            return true;
        }
    }
}
