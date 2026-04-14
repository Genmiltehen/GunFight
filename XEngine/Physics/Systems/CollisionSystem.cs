using OpenTK.Mathematics;
using System.Diagnostics;
using System.Drawing;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Physics.ColliderShapes;
using XEngine.Core.Physics.CollisionDetector;
using XEngine.Core.Physics.Utils;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace XEngine.Core.Physics.Systems
{
    public class CollisionSystem : IGameSystem
    {
        public int Priority => 400;
        private readonly ICollisionDetector _detector;
        private readonly CollisionManifoldGenerator _generator;

        private const float BAUMGARTE_BETA = 0.1f;
        private const float SLOP_FACTOR = 2f;
        private const float E = 0.0f;
        private const int ResolveCycles = 20;

        public CollisionSystem(ICollisionDetector detector)
        {
            _detector = detector;
            _generator = new();
        }

        public void Update(Scene _scene, float _dt)
        {
            //for (int i = 0; i < ResolveCycles; i++)
                foreach (var m in _detector.GetManifolds(_scene, _generator.Check))
                    ForceResolution(m, _dt);
            //for (int i = 0; i < ResolveCycles; i++)
            //    foreach (var m in _detector.GetManifolds(_scene, _generator.Check))
            //    {
            //        if (m.physA.rb.IsStatic && m.physB.rb.IsStatic) continue;
            //        OverlapResolution(m);
            //        //ForceResolution(m, _dt);
            //    }
        }

        //private static void OverlapResolution(CollisionManifold m)
        //{
        //    var a = m.physA;
        //    var b = m.physB;

        //    float totalInverseMass = a.rb.InvMass + b.rb.InvMass;
        //    if (totalInverseMass <= 0) return;

        //    Vector2 l_sep = m.Normal * (m.Depth / totalInverseMass);
        //    a.tr.Position2D -= l_sep * a.rb.InvMass;
        //    b.tr.Position2D += l_sep * b.rb.InvMass;
        //}

        private static void ForceResolution(CollisionManifold m, float dt)
        {
            var a = m.physA;
            var b = m.physB;
            foreach (var PoC in m.contacts)
            {
                Vector2 r_A = PoC.point - a.tr.Position2D;
                Vector2 r_B = PoC.point - b.tr.Position2D;
                Vector2 v_rel = b.rb.TotalVelAtPoint(r_B) - a.rb.TotalVelAtPoint(r_A);

                float contactVelocity = Vector2.Dot(v_rel, m.Normal);
                if (contactVelocity > 0) return;

                float r_APerpN = MathUtils.Cross2D(r_A, m.Normal);
                float r_BPerpN = MathUtils.Cross2D(r_B, m.Normal);

                float denom = a.rb.InvMass + b.rb.InvMass +
                  (r_APerpN * r_APerpN) * a.rb.InvInertia +
                  (r_BPerpN * r_BPerpN) * b.rb.InvInertia;

                float bias = (BAUMGARTE_BETA) / dt * Math.Max(0, PoC.depth - SLOP_FACTOR);
                float j = -((1 + E) * contactVelocity - bias) / denom;
                //if (j < 0) continue;
                Vector2 F = j * m.Normal / dt;

                //Debug.WriteLine($"{PoC.depth,10:F4}, {j,10:F4} {bias,10:F4}, {contactVelocity,10:F4}, {denom,8:F3}");
                a.rb.AddForceAtPoint(r_A, -F);
                b.rb.AddForceAtPoint(r_B, F);
            }
        }
    }
}
