using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using XEngine.Core.Input.InputAxis;
using XEngine.Core.Physics.Collision.Shapes;
using XEngine.Core.Physics.MathStructs;
using XEngine.Core.Utils;
using static System.Windows.Forms.DataFormats;

namespace XEngine.Core.Physics.Collision.NarrowPhase
{
    public delegate CollisionManifold? CollisionChecker(CollisionObject a, CollisionObject b);

    internal static class CollisionMatrix
    {
        private static readonly CollisionChecker[,] _matrix;

        static CollisionMatrix()
        {
            int count = Enum.GetValues(typeof(ColliderType)).Length;
            _matrix = new CollisionChecker[count, count];

            Register(ColliderType.Box, ColliderType.Box, BoxBox);
            //_generator.Register(ColliderType.Box, ColliderType.Capsule, BoxCapsule);
            Register(ColliderType.Capsule, ColliderType.Capsule, CapsuleCapsule);
        }

        private static void Register(ColliderType a, ColliderType b, CollisionChecker func)
        {
            _matrix[(int)a, (int)b] = func;

            if (a != b)
            {
                _matrix[(int)b, (int)a] = (entityB, entityA) =>
                {
                    var _m = func(entityA, entityB);
                    if (_m.HasValue)
                    {
                        var m = _m.Value;
                        m.Normal = -m.Normal;
                        (m.coA, m.coB) = (m.coB, m.coA);
                        return m;
                    }
                    return null;
                };
            }
        }

        public static CollisionManifold? Check(CollisionObject a, CollisionObject b)
        {
            var typeA = a.col.Shape.ColliderType;
            var typeB = b.col.Shape.ColliderType;

            if (a.rb.IsStatic && b.rb.IsStatic) return null;

            return _matrix[(int)typeA, (int)typeB]?.Invoke(a, b) ?? null;
        }

        private static CollisionManifold? BoxBox(CollisionObject a, CollisionObject b)
        {
            if (a.col.Shape is not BoxCollider boxA || b.col.Shape is not BoxCollider boxB) return null;

            var pointsA = boxA.GetCorners(a.tr);
            var pointsB = boxB.GetCorners(b.tr);

            if (!SATMethod.TestSAT(pointsA, pointsB, out Vector2 normal, out float depth)) return null;

            Segment refEdge = SATMethod.GetBestEdge(pointsA, normal);
            Segment incEdge = SATMethod.GetBestEdge(pointsB, -normal);

            bool flipped = true;

            if (MathF.Abs(Vector2.Dot(refEdge.Direction, normal)) <= MathF.Abs(Vector2.Dot(incEdge.Direction, normal)))
                flipped = false;
            else (incEdge, refEdge) = (refEdge, incEdge);

            Vector2 refDir = refEdge.Direction;

            float offsetStart = Vector2.Dot(refDir, refEdge.start);
            var clipped = SATMethod.Clip(incEdge, refDir, offsetStart);
            if (!clipped.HasValue) return null;

            float offsetEnd = Vector2.Dot(refDir, refEdge.end);
            clipped = SATMethod.Clip(clipped.Value, -refDir, -offsetEnd);
            if (!clipped.HasValue) return null;
            var def_clipped = clipped.Value;    

            Vector2 refNormal = MathUtils.PerpTowards(refDir, flipped ? -normal : normal);
            float maxDepthOffset = Vector2.Dot(refNormal, refEdge.start);
            CollisionManifold manifold = new() { coA = a, coB = b, Normal = normal };

            float d1 = Vector2.Dot(refNormal, def_clipped.start) - maxDepthOffset;
            if (d1 <= 0)
            {
                manifold.contact1 = new ContactPoint { point = def_clipped.start, depth = -d1 };
                manifold.contactCount++;
            }

            float d2 = Vector2.Dot(refNormal, def_clipped.end) - maxDepthOffset;
            if (d2 <= 0)
            {
                ContactPoint cp = new() { point = def_clipped.end, depth = -d2 };
                if (manifold.contactCount == 0)
                    manifold.contact1 = cp;
                else
                    manifold.contact2 = cp;
                manifold.contactCount++;
            }

            return manifold.contactCount > 0 ? manifold : null;
        }

        private const float PARALLEL_THRESHOLD = 0.99999f;
        private static CollisionManifold? CapsuleCapsule(CollisionObject a, CollisionObject b)
        {
            if (a.col.Shape is not CapsuleCollider cp1) return null;
            if (b.col.Shape is not CapsuleCollider cp2) return null;
            cp1!.GetSegment(a.tr, out Segment seg1);
            cp2!.GetSegment(b.tr, out Segment seg2);

            float sumR = cp1.Radius + cp2.Radius;
            float deltaR_2 = (cp1.Radius - cp2.Radius) / 2;

            Vector2 normal;
            Vector2 axis1 = seg1.Direction;
            Vector2 axis2 = seg2.Direction;

            MathUtils.ClosestPointsBetweenSegments(seg1, seg2, out Vector2 p1, out Vector2 p2);
            float dist = (p2 - p1).Length;
            if (dist >= sumR) return null;

            if (MathF.Abs(Vector2.Dot(axis1, axis2)) > PARALLEL_THRESHOLD)
            {
                MathUtils.PolygonProjectionBounds(axis1, seg1.AsArray(), out float min1, out float max1);
                MathUtils.PolygonProjectionBounds(axis1, seg2.AsArray(), out float min2, out float max2);
                Vector2 translation = MathUtils.DirFromLineToPoint(seg2.start, seg1.start, axis1);
                float LT = translation.Length;
                float depth = sumR - LT;
                if (depth <= 0) return null;

                float Min = MathF.Max(min1, min2);
                float Max = MathF.Min(max1, max2);

                if (Max > Min)
                {
                    float len = deltaR_2 + LT / 2;
                    normal = translation.Normalized();
                    Vector2 pointL = a.tr.Position2D + axis1 * Min + normal * len;
                    Vector2 pointR = a.tr.Position2D + axis1 * Max + normal * len;

                    return new CollisionManifold
                    {
                        coA = a,
                        coB = b,
                        Normal = normal,
                        contact1 = new() { point = pointL, depth = depth },
                        contact2 = new() { point = pointR, depth = depth },
                        contactCount = 2
                    };
                }
            }

            normal = dist > 0 ? (p2 - p1) / dist : Vector2.UnitY;
            return new CollisionManifold
            {
                coA = a,
                coB = b,
                Normal = normal,
                contact1 = new() { point = (p1 + p2) / 2 + normal * deltaR_2, depth = sumR - dist },
                contactCount = 1
            };

        }
    }
}
