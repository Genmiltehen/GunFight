using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Physics.Collision.Detection;
using XEngine.Core.Physics.Collision.NarrowPhase;
using XEngine.Core.Physics.Components;
using XEngine.Core.Physics.MathStructs;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace XEngine.Core.Physics.Dynamics
{
    public class CollisionSystem : IGameSystem
    {
        public int Priority => 400;
        private readonly IBroadPhase _detector;

        private const float BAUMGARTE_BETA = 0.1f;
        private const float SLOP_FACTOR = 3f;
        private const int ResolveCycles = 20;

        // TODO: material component
        private const float ELASTICITY = 0.0f;
        private const float FRICTION_MU = 1f;

        private readonly HashSet<int> _resolve_ids = [];

        public CollisionSystem(IBroadPhase detector)
        {
            _detector = detector;
        }

        public void Update(Scene _scene, float _dt)
        {
            var manifolds = _detector.GetPotentialPairs(_scene)
                .Select(pair => CollisionMatrix.Check(pair.Item1, pair.Item2))
                .Where(m => m.HasValue)
                .Select(m => m.Value);

            for (int i = 0; i < ResolveCycles; i++)
            {
                _resolve_ids.Clear();
                foreach (var pair in _detector.GetPotentialPairs(_scene))
                {
                    var m = CollisionMatrix.Check(pair.Item1, pair.Item2);
                    if (m.HasValue) ForceResolution(m.Value, _dt, _resolve_ids);
                }
                foreach (var e in _scene.IterateByIds(_resolve_ids)) e.Get<Rigidbody>()!.FlushImpulse();
            }
            for (int i = 0; i < ResolveCycles; i++)
                foreach (var pair in _detector.GetPotentialPairs(_scene))
                {
                    var m = CollisionMatrix.Check(pair.Item1, pair.Item2);
                    if (m.HasValue) OverlapResolution(m.Value);
                }
        }

        private static void OverlapResolution(CollisionManifold m)
        {
            foreach (var poc in m.contacts) OverlapResolutionPerPoC(m, poc);
        }

        private static void OverlapResolutionPerPoC(CollisionManifold m, ContactPoint PoC)
        {
            var a = m.coA;
            var b = m.coB;

            float C = Math.Max(0, PoC.depth - SLOP_FACTOR);
            if (C <= 0) return;
            float K_inv = m.CalculateK_inv(PoC, m.Normal, out float rAperpN, out float rBperpN);
            if (K_inv <= 0) return;
            float lambda = C / K_inv;

            a.tr.Position2D -= lambda * m.Normal * a.rb.InvMass;
            b.tr.Position2D += lambda * m.Normal * b.rb.InvMass;
            a.tr.Rotation -= lambda * rAperpN * a.rb.InvInertia;
            b.tr.Rotation += lambda * rBperpN * b.rb.InvInertia;
        }

        private static void ForceResolution(CollisionManifold m, float dt, HashSet<int> ids)
        {
            ids.Add(m.coA.entityId);
            ids.Add(m.coB.entityId);
            foreach (var poc in m.contacts) ForceResolvePerPoC(m, poc, dt);
        }

        private static void ForceResolvePerPoC(CollisionManifold m, ContactPoint PoC, float dt)
        {
            var a = m.coA;
            var b = m.coB;

            Vector2 r_A = PoC.point - a.tr.Position2D;
            Vector2 r_B = PoC.point - b.tr.Position2D;
            Vector2 v_rel = b.rb.TotalVelAtPoint(r_B) - a.rb.TotalVelAtPoint(r_A);

            float contactVelocity = Vector2.Dot(v_rel, m.Normal);
            if (contactVelocity > 0) return;

            // REACTION
            float C = Math.Max(0, PoC.depth - SLOP_FACTOR);
            if (C <= 0) return;
            float K_inv = m.CalculateK_inv(PoC, m.Normal, out float _, out float _);
            if (K_inv <= 0) return;

            // BAUMGARTE STABILIZATION
            float bias = BAUMGARTE_BETA / dt * C;
            float j = -((1 + ELASTICITY) * contactVelocity - bias) / K_inv;
            if (j < 0) return;

            Vector2 J = j * m.Normal;
            a.rb.ApplyImpulseAtPoint(r_A, -J);
            b.rb.ApplyImpulseAtPoint(r_B, J);

            // FRICTION
            Vector2 tangent = MathUtils.LeftPerp(m.Normal);
            float v_rel_tan = Vector2.Dot(v_rel, tangent);
            float K_tangent = m.CalculateK_inv(PoC, tangent, out float _, out float _);
            float jt = -v_rel_tan / K_tangent;
            float maxFriction = FRICTION_MU * j;
            jt = Math.Clamp(jt, -maxFriction, maxFriction);

            Vector2 J_friction = jt * tangent;
            a.rb.ApplyImpulseAtPoint(r_A, -J_friction);
            b.rb.ApplyImpulseAtPoint(r_B, J_friction);
        }
    }
}
