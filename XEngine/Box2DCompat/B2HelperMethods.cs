using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Box2D.NET.B2MathFunction;

namespace XEngine.Core.Box2DCompat
{
    public static class B2HelperMethods
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
                float d = w / 2 - r;
                p1 = b2Add(center, new B2Vec2(d, 0));
                p2 = b2Add(center, new B2Vec2(-d, 0));
            }
            else
            {
                r = w / 2;
                float d = h / 2 - r;

                p1 = b2Add(center, new B2Vec2(0, d));
                p2 = b2Add(center, new B2Vec2(0, -d));

            }

            return new B2Capsule()
            {
                center1 = p1,
                center2 = p2,
                radius = r
            };
        }
    }
}
