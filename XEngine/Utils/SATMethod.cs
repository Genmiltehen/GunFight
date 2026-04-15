using OpenTK.Mathematics;

namespace XEngine.Core.Utils
{
    internal static class SATMethod
    {
        public static Segment GetBestEdge(Vector2[] points, Vector2 normal)
        {
            int index = 0;
            float maxDot = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                float dot = Vector2.Dot(points[i], normal);
                if (dot > maxDot) { maxDot = dot; index = i; }
            }

            Vector2 v = points[index];
            Vector2 vLeft = points[(index - 1 + points.Length) % points.Length];
            Vector2 vRight = points[(index + 1) % points.Length];

            Vector2 l = (v - vLeft).Normalized();
            Vector2 r = (v - vRight).Normalized();
            return Vector2.Dot(r - l, normal) > 0 ? new() { start = vLeft, end = v } : new() { start = v, end = vRight };
        }

        public static bool TestSAT(Vector2[] pointsA, Vector2[] pointsB, out Vector2 normal, out float depth)
        {
            normal = Vector2.Zero;
            depth = float.MaxValue;
            IEnumerable<Vector2> allAxes = MathUtils.GetPolygonNormals(pointsA).Concat(MathUtils.GetPolygonNormals(pointsB));

            foreach (Vector2 axis in allAxes)
            {
                MathUtils.PolygonProjectionBounds(axis, pointsA, out float minA, out float maxA);
                MathUtils.PolygonProjectionBounds(axis, pointsB, out float minB, out float maxB);
                if (minA > maxB || minB > maxA) return false;

                float overlap = Math.Min(maxA, maxB) - Math.Max(minA, minB);
                if (overlap < depth)
                {
                    depth = overlap;
                    normal = axis;
                }
            }

            Vector2 centerA = MathUtils.CenterOfMass(pointsA);
            Vector2 centerB = MathUtils.CenterOfMass(pointsB);
            if (Vector2.Dot(normal, centerB - centerA) < 0) normal = -normal;
            return true;
        }

        public static Segment? Clip(Segment seg, Vector2 n, float offset)
        {
            float dStart = Vector2.Dot(n, seg.start);
            float dEnd = Vector2.Dot(n, seg.end);
            if (dStart < offset && dEnd < offset) return null;
            if (dStart >= offset && dEnd >= offset) return seg;

            Vector2 intersect = seg.Lerp((dStart - offset) / (dStart - dEnd));

            return new Segment()
            {
                start = dStart >= offset ? seg.start : intersect,
                end = dEnd >= offset ? seg.end : intersect
            };
        }
    }
}