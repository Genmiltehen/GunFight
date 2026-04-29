using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            using (lb.TraceLine(closed: true))
            {
                switch (B2Shapes.b2Shape_GetType(shapeId))
                {
                    case B2ShapeType.b2_circleShape:
                        TraceSegment(lb, B2Shapes.b2Shape_GetSegment(shapeId), transform);
                        break;
                    case B2ShapeType.b2_capsuleShape:
                        TraceCapsule(lb, B2Shapes.b2Shape_GetCapsule(shapeId), transform);
                        break;
                    case B2ShapeType.b2_segmentShape:
                        TraceCircle(lb, B2Shapes.b2Shape_GetCircle(shapeId), transform);
                        break;
                    case B2ShapeType.b2_polygonShape:
                        TracePolygon(lb, B2Shapes.b2Shape_GetPolygon(shapeId), transform);
                        break;
                }
            }
        }

        private static void CalcArc(ref B2Vec2[] points, float radius, float startAngle, float endAngle)
        {
            float t, ang;
            for (int i = 0; i < points.Length; i++)
            {
                t = (float)i / (points.Length - 1);
                ang = float.Lerp(startAngle, endAngle, t);
                points[i] = b2RotateVector(b2MakeRot(ang), new B2Vec2(radius, 0));
            }
        }

        private static void TraceSegment(LineBatcher lb, B2Segment seg, B2Transform tr)
        {
            lb.AddPoint(b2TransformPoint(tr, seg.point1));
            lb.AddPoint(b2TransformPoint(tr, seg.point2));
        }

        private readonly static int PolyRes = 8;
        private static void TracePolygon(LineBatcher lb, B2Polygon poly, B2Transform tr)
        {
            B2Vec2[] points = new B2Vec2[PolyRes + 1];
            B2Vec2 prev = b2TransformPoint(tr, poly.vertices[poly.count - 1]);
            B2Vec2 curr = b2TransformPoint(tr, poly.vertices[0]);
            B2Vec2 next = b2TransformPoint(tr, poly.vertices[1]);
            GetAngles(prev, curr, next, out float start, out float end);
            CalcArc(ref points, poly.radius, start, end);
            for (int i = 0; i <= PolyRes; i++) lb.AddPoint(b2Add(points[i], curr));

            for (int i = 1; i < poly.count; i++)
            {
                prev = curr;
                curr = next;
                next = b2TransformPoint(tr, poly.vertices[(i + 1) % poly.count]);
                GetAngles(prev, curr, next, out start, out end);
                CalcArc(ref points, poly.radius, start, end);
                for (int j = 0; j <= PolyRes; j++) lb.AddPoint(b2Add(points[j], curr));
            }
        }

        private static void GetAngles(B2Vec2 a, B2Vec2 b, B2Vec2 c, out float start, out float end)
        {
            start = B2MathFunction.b2Atan2(b.Y - c.Y, b.X - c.X);
            end = B2MathFunction.b2Atan2(b.Y - a.Y, b.X - a.X);
            if (end < start) end += MathF.Tau;
        }

        private readonly static int CapRes = 8;
        private static void TraceCapsule(LineBatcher lb, B2Capsule capsule, B2Transform tr)
        {
            float capAng = b2Atan2(
                capsule.center1.Y - capsule.center2.Y,
                capsule.center1.X - capsule.center2.X
            ) - MathF.PI / 2;

            B2Vec2[] points = new B2Vec2[CapRes + 1];
            CalcArc(ref points, capsule.radius, capAng, capAng + MathF.PI);
            for (int i = 0; i <= CapRes; i++) lb.AddPoint(b2Add(points[i], b2TransformPoint(tr, capsule.center1)));
            capAng += MathF.PI;
            CalcArc(ref points, capsule.radius, capAng, capAng + MathF.PI);
            for (int i = 0; i <= CapRes; i++) lb.AddPoint(b2Add(points[i], b2TransformPoint(tr, capsule.center2)));
        }

        private readonly static int CircRes = 16;
        private static void TraceCircle(LineBatcher lb, B2Circle circle, B2Transform tr)
        {
            B2Vec2[] points = new B2Vec2[CircRes + 1];
            CalcArc(ref points, circle.radius, 0, MathF.Tau);
            for (int i = 0; i <= CircRes; i++) lb.AddPoint(b2Add(points[i], b2TransformPoint(tr, circle.center)));
        }
    }
}
