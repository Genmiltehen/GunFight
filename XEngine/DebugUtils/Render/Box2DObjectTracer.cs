using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Graphics.OpenGL;

using static Box2D.NET.B2MathFunction;

namespace XEngine.Core.DebugUtils.Render
{
    internal static class Box2DObjectTracer
    {
        public static void TraceBody(B2BodyId bodyId, LineBatcher lb)
        {
            B2Transform transform = B2Bodies.b2Body_GetTransform(bodyId);

            int shapeCount = B2Bodies.b2Body_GetShapeCount(bodyId);
            Span<B2ShapeId> shapeIds = new B2ShapeId[shapeCount];
            B2Bodies.b2Body_GetShapes(bodyId, shapeIds, shapeCount);

            foreach (var shapeId in shapeIds) TraceShape(shapeId, transform, lb);
        }

        private static void TraceShape(B2ShapeId shapeId, B2Transform transform, LineBatcher lb)
        {
            switch (B2Shapes.b2Shape_GetType(shapeId))
            {
                case B2ShapeType.b2_polygonShape:
                    TracePolygon(lb, B2Shapes.b2Shape_GetPolygon(shapeId), transform);
                    break;
                case B2ShapeType.b2_capsuleShape:
                    TraceCapsule(lb, B2Shapes.b2Shape_GetCapsule(shapeId), transform);
                    break;
                case B2ShapeType.b2_circleShape:
                    TraceCircle(lb, B2Shapes.b2Shape_GetCircle(shapeId), transform);
                    break;
            }
        }

        private static void TracePolygon(LineBatcher lb, B2Polygon poly, B2Transform tr)
        {
            using (lb.TraceLine(closed: true))
            {
                for (int i = 0; i < poly.count; i++)
                {
                    lb.AddPoint(b2TransformPoint(tr, poly.vertices[i]));
                }
            }
        }

        private readonly static int CapRes = 8;
        private static void TraceCapsule(LineBatcher lb, B2Capsule capsule, B2Transform tr)
        {
            using (lb.TraceLine(closed: true))
            {
                float capAng = b2Atan2(
                    capsule.center2.Y - capsule.center1.Y,
                    capsule.center2.X - capsule.center1.X
                ) + MathF.PI / 2;
                for (int i = 0; i <= CapRes; i++)
                {
                    float ang = MathF.PI / CapRes * i + capAng;
                    B2Vec2 point = b2RotateVector(b2MakeRot(ang), new B2Vec2(capsule.radius, 0));
                    lb.AddPoint(b2TransformPoint(tr, b2Add(point, capsule.center1)));
                }
                capAng += MathF.PI;
                for (int i = 0; i <= CapRes; i++)
                {
                    float ang = MathF.PI / CapRes * i + capAng;
                    B2Vec2 point = b2RotateVector(b2MakeRot(ang), new B2Vec2(capsule.radius, 0));
                    lb.AddPoint(b2TransformPoint(tr, b2Add(point, capsule.center2)));
                }
            }
        }

        private readonly static int CircRes = 16;
        private static void TraceCircle(LineBatcher lb, B2Circle circle, B2Transform tr)
        {
            using (lb.TraceLine(closed: true))
            {
                for (int i = 0; i <= CircRes; i++)

                {
                    float ang = MathF.Tau / CircRes * i;
                    B2Vec2 point = b2RotateVector(b2MakeRot(ang), new B2Vec2(circle.radius, 0));
                    lb.AddPoint(b2TransformPoint(tr, b2Add(point, circle.center)));
                }
            }
        }
    }
}
