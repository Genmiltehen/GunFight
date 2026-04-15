using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Physics.Components;
using XEngine.Core.Scenery;

namespace XEngine.Core.Physics.Dynamics
{
    public class IntegrationSystem : IGameSystem
    {
        public int Priority => 450;

        public Vector2 Gravity;

        public IntegrationSystem(Vector2 gravity)
        {
            Gravity = gravity;
        }

        public void Update(Scene _scene, float _dt)
        {
            foreach (var (_, tr, rb) in _scene.Query<TransformComp, Rigidbody>())
            {
                if (rb.IsStatic) continue;

                if (rb.InvMass != 0)
                {
                    Vector2 gravityForce = Gravity * rb.GravityScale * rb.Mass;
                    rb.AddForce(gravityForce);
                }

                SolveVerlet(tr, rb, _dt);
            }
        }

        private static void SolveVerlet(TransformComp tr, Rigidbody rb, float dt)
        {
            Vector2 acceleration = rb.FlushForce() * rb.InvMass;
            rb.Velocity += acceleration * dt;
            rb.Velocity *= rb.Drag;
            tr.Position2D += rb.Velocity * dt;

            float angularAcc = rb.FlushTorque() * rb.InvInertia;
            rb.AngVelocity += angularAcc * dt;
            rb.AngVelocity *= rb.AngDrag;
            tr.Rotation += rb.AngVelocity * dt;
        }
    }
}
