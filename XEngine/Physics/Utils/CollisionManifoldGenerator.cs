using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using XEngine.Core.Input.InputAxis;
using XEngine.Core.Physics.ColliderShapes;
using XEngine.Core.Utils;
using static System.Windows.Forms.DataFormats;

namespace XEngine.Core.Physics.Utils
{
    public class CollisionManifoldGenerator
    {
        private readonly CollisionChecker[,] _matrix;

        public CollisionManifoldGenerator()
        {
            int count = Enum.GetValues(typeof(ColliderType)).Length;
            _matrix = new CollisionChecker[count, count];

            Register(ColliderType.Box, ColliderType.Box, BoxBox);
            //_generator.Register(ColliderType.Box, ColliderType.Capsule, BoxCapsule);
            Register(ColliderType.Capsule, ColliderType.Capsule, CapsuleCapsule);
        }

        public void Register(ColliderType a, ColliderType b, CollisionChecker func)
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
                        (m.physA, m.physB) = (m.physB, m.physA);
                        return _m;
                    }
                    return null;
                };
            }
        }

        public CollisionManifold? Check(PhysicsEntityData a, PhysicsEntityData b)
        {
            var typeA = a.col.Shape.ColliderType;
            var typeB = b.col.Shape.ColliderType;

            if (a.rb.IsStatic && b.rb.IsStatic) return null;

            return _matrix[(int)typeA, (int)typeB]?.Invoke(a, b) ?? null;
        }

        private static CollisionManifold? BoxBox(PhysicsEntityData a, PhysicsEntityData b)
        {
            //if (a.col.Shape is not BoxCollider bc1) return null;
            //if (b.col.Shape is not BoxCollider bc2) return null;

            //if (!MathUtils.TestSAT(bc1.GetCorners(a.tr), bc1.GetCorners(a.tr), out Vector2 normal)) return null;

            //return new CollisionManifold
            //{
            //    physA = a,
            //    physB = b,
            //    Normal = normal,
            //    Depth = sumR - dist,
            //    ContactPoint = (p1 + p2) / 2 + normal * (bc1.Radius - bc2.Radius) / 2
            //};
            return null;
        }

        private const float PARALLEL_THRESHOLD = 0.9999f;
        private static CollisionManifold? CapsuleCapsule(PhysicsEntityData a, PhysicsEntityData b)
        {
            if (a.col.Shape is not CapsuleCollider cp1) return null;
            if (b.col.Shape is not CapsuleCollider cp2) return null;

            cp1!.GetPoints(a.tr, out Vector2 Start1, out Vector2 End1);
            cp2!.GetPoints(b.tr, out Vector2 Start2, out Vector2 End2);

            Vector2 dir1 = End1 - Start1;
            Vector2 dir2 = End2 - Start2;

            Vector2 axis1 = dir1 / cp1.Length;
            Vector2 axis2 = dir2 / cp2.Length;

            float sumR = cp1.Radius + cp2.Radius;

            if (MathF.Abs(Vector2.Dot(axis1, axis2)) > PARALLEL_THRESHOLD)
            {
                MathUtils.PolygonBoundsAlongAxis(axis1, [Start1, End1], out float min1, out float max1);
                MathUtils.PolygonBoundsAlongAxis(axis1, [Start2, End2], out float min2, out float max2);
                Vector2 dir = MathUtils.DirFromLineToPoint(Start2, Start1, axis1);
                float depth = sumR - dir.Length;
                if (depth <= 0) return null;

                Vector2 normal = dir.Normalized();
                Vector2 point1 = a.tr.Position2D + axis1 * MathF.Max(min1, min2) + normal * cp1.Radius;
                Vector2 point2 = a.tr.Position2D + axis1 * MathF.Min(max1, max2) + normal * cp1.Radius;

                return new CollisionManifold
                {
                    physA = a, physB = b, Normal = normal,
                    contacts = [ 
                        new() { point = point1, depth = depth },
                        new() { point = point2, depth = depth },
                    ],
                };
            }
            else
            {
                MathUtils.ClosestPointsBetweenSegments(Start1, dir1, Start2, dir2, out Vector2 p1, out Vector2 p2);

                float distSq = (p2 - p1).LengthSquared;
                if (distSq >= sumR * sumR) return null;

                float dist = MathF.Sqrt(distSq);
                Vector2 normal = dist > 0 ? (p2 - p1) / dist : Vector2.UnitY;

                return new CollisionManifold
                {
                    physA = a, physB = b, Normal = normal,
                    contacts = [ new() {
                        point=(p1 + p2) / 2 + normal * (cp1.Radius - cp2.Radius) / 2,
                        depth = sumR - dist
                    } ],
                };
            }
        }
    }
}
