using Microsoft.VisualBasic;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
            Register(ColliderType.Box, ColliderType.Capsule, BoxCapsule);
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

            if (!SATMethod.TestSAT(pointsA, pointsB, out Vector2 normal)) return null;

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

            List<ContactPoint> res = [];

            float d1 = Vector2.Dot(refNormal, def_clipped.start) - maxDepthOffset;
            if (d1 <= 0) res.Add(new ContactPoint { point = def_clipped.start, depth = -d1 });

            float d2 = Vector2.Dot(refNormal, def_clipped.end) - maxDepthOffset;
            if (d2 <= 0) res.Add(new() { point = def_clipped.end, depth = -d2 });

            manifold.contacts = [.. res];
            return res.Count == 0 ? null : manifold;
        }

        private static CollisionManifold? BoxCapsule(CollisionObject cap, CollisionObject box)
        {
            CapsuleCollider cap_col;
            BoxCollider box_col;
            if (cap.col.Shape is CapsuleCollider cc1 && box.col.Shape is BoxCollider cc2)
            {
                cap_col = cc1;
                box_col = cc2;
            }
            else if (cap.col.Shape is BoxCollider cc3 && box.col.Shape is CapsuleCollider cc4)
            {
                cap_col = cc4;
                box_col = cc3;
                (cap, box) = (box, cap);
            }
            else return null;

            var cap_seg = cap_col.GetSegment(cap.tr);
            Vector2 box_c = box.tr.Position2D;
            Vector2 segPoint = MathUtils.LineProjectionClipped(box_c, cap_seg);
            Vector2 axis, delta = segPoint - box_c;
            MathUtils.CalcAxes(box.tr.Rotation, out Vector2 axisX, out Vector2 axisY);
            axisX = MathUtils.FlipTowards(axisX, delta);
            axisY = MathUtils.FlipTowards(axisY, delta);
            float distX = Vector2.Dot(delta, axisX) - box_col.HalfSize.X;
            float distY = Vector2.Dot(delta, axisY) - box_col.HalfSize.Y;
            if (distX <= 0.0f) axis = axisY;
            else if (distY <= 0.0f) axis = axisX;
            else axis = MathF.Abs(distX) < MathF.Abs(distY) ? axisX : axisY;
            var box_seg = SATMethod.GetBestEdge(box_col.GetCorners(box.tr), axis);

            // normal: box -> cap
            var contacts = GetCapsuleContactPoints(box_seg, 0, cap_seg, cap_col.Radius, out Vector2 normal);

            return contacts.Length == 0 ? null : new()
            {
                coA = box,
                coB = cap,
                Normal = normal,
                contacts = contacts,
            };
        }

        private static CollisionManifold? CapsuleCapsule(CollisionObject a, CollisionObject b)
        {
            if (a.col.Shape is not CapsuleCollider cp1) return null;
            if (b.col.Shape is not CapsuleCollider cp2) return null;
            Segment seg1 = cp1!.GetSegment(a.tr);
            Segment seg2 = cp2!.GetSegment(b.tr);

            var contacts = GetCapsuleContactPoints(seg1, cp1.Radius, seg2, cp2.Radius, out Vector2 normal);
            return contacts.Length == 0 ? null : new()
            {
                coA = a,
                coB = b,
                Normal = normal,
                contacts = contacts,
            };
        }

        private const float PARALLEL_THRESHOLD = 0.99999f;
        private static ContactPoint[] GetCapsuleContactPoints(Segment seg1, float R1, Segment seg2, float R2, out Vector2 normal)
        {
            normal = Vector2.UnitY;
            float sumR = R1 + R2;
            float depth, len;

            Vector2 axis1 = seg1.Direction;
            Vector2 axis2 = seg2.Direction;

            MathUtils.ClosestPointsBetweenSegments(seg1, seg2, out Vector2 p1, out Vector2 p2);
            float dist = (p2 - p1).Length;
            if (dist >= sumR) return [];

            if (MathF.Abs(Vector2.Dot(axis1, axis2)) > PARALLEL_THRESHOLD)
            {
                MathUtils.PolygonProjectionBounds(axis1, seg1.AsArray(), out float min1, out _);
                MathUtils.PolygonProjectionBounds(axis1, seg2.AsArray(), out float min2, out float max2);
                Vector2 translation = seg2.start - MathUtils.LineProjection(seg2.start, seg1);
                float LT = translation.Length;

                //Debug.WriteLine($"{translation} {seg2.start} {MathUtils.SnapToLine(seg2.start, seg1)}");

                depth = sumR - LT;
                if (depth <= 0) return [];

                max2 = (max2 - min1) / seg1.Length;
                min2 = (min2 - min1) / seg1.Length;
                float Min = MathF.Max(0, min2);
                float Max = MathF.Min(1, max2);

                if (Max > Min)
                {
                    len = R2;
                    normal = translation.Normalized();
                    Vector2 pointL = seg1.Lerp(Min) - normal * len;
                    Vector2 pointR = seg1.Lerp(Max) - normal * len;

                    return [
                        new() { point = pointL, depth = depth },
                        new() { point = pointR, depth = depth },
                    ];
                }
            }

            depth = sumR - dist;
            len = R1 - depth / 2;
            normal = dist > 0 ? (p2 - p1) / dist : Vector2.UnitY;
            return [new() { point = p1 + normal * len, depth = depth }];
        }
    }
}
