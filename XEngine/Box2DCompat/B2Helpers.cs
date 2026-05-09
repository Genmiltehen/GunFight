using Box2D.NET;
using static Box2D.NET.B2MathFunction;
using static Box2D.NET.B2Bodies;
using static Box2D.NET.B2Shapes;
using XEngine.Core.Base;

namespace XEngine.Core.Box2DCompat
{
    public static class B2Helpers
    {
        public static B2Capsule MakeCapsule(B2AABB aabb)
        {
            var w = aabb.upperBound.X - aabb.lowerBound.X;
            var h = aabb.upperBound.Y - aabb.lowerBound.Y;
            var center = b2Lerp(aabb.lowerBound, aabb.upperBound, 0.5f);

            B2Vec2 p1, p2;
            float r;
            if (w > h)
            {
                r = h / 2;
                B2Vec2 v = new(w / 2 - r, 0);
                p1 = center + v;
                p2 = center - v;
            }
            else
            {
                r = w / 2;
                B2Vec2 v = new(0, h / 2 - r);
                p1 = center + v;
                p2 = center - v;

            }

            return new B2Capsule()
            {
                center1 = p1,
                center2 = p2,
                radius = r
            };
        }

        public static bool CheckFlag(B2ShapeId id, ulong flag)
        {
            return (B2Shapes.b2Shape_GetFilter(id).categoryBits & flag) == flag;
        }
    
        public static bool TryFetchEntity(B2ShapeId id, out Entity entity)
        {
            var e = b2Body_GetUserData(b2Shape_GetBody(id)).GetRef<UserData>()?.HostBody.Owner;
            entity = e ?? default!;
            return e != null;
        }
    }
}
