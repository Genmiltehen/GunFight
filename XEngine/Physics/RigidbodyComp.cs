using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Utils;

namespace XEngine.Core.Physics
{
    public class RigidbodyComp : GameComponent
    {
        public float Drag = 0.99f;
        public Vector2 Velocity = Vector2.Zero;
        private Vector2 _accForce = Vector2.Zero;
        public float InvMass;
        public float Mass
        {
            get => InvMass <= 0 ? float.PositiveInfinity : 1f / InvMass;
            set => InvMass = value <= 0 ? 0 : 1f / value;
        }

        public float AngDrag = 0.99f;
        public float AngVelocity = 0;
        private float _accTorque = 0;
        public float InvInertia;
        public float Inertia
        {
            get => InvInertia <= 0 ? float.PositiveInfinity : 1f / InvInertia;
            set => InvInertia = value <= 0 ? 0 : 1f / value;
        }

        public bool IsStatic => _static;
        private bool _static = false;
        public float GravityScale;

        public RigidbodyComp Init(float mass, float inertia)
        {
            Mass = mass;
            Inertia = inertia;
            return this;
        }

        public RigidbodyComp SetStatic()
        {
            _static = true;
            InvInertia = 0;
            InvMass = 0;
            return this;
        }

        /// <summary>
        /// Calulates total velocity of point relative to the center
        /// </summary>
        /// <param name="r">point at which velecity is calculated; (0,0) is center</param>
        /// <returns></returns>
        public Vector2 TotalVelAtPoint(Vector2 r)
        {
            return Velocity + new Vector2(-AngVelocity * r.Y, AngVelocity * r.X);
        }

        public void AddForceAtPoint(Vector2 r, Vector2 F)
        {
            AddForce(F);
            AddTorque(MathUtils.Cross2D(r, F));
        }

        public void AddForce(Vector2 force)
        {
            _accForce += force;
        }

        public Vector2 FlushForce()
        {
            var res = _accForce;
            _accForce = Vector2.Zero;
            return res;
        }

        public void AddTorque(float torque)
        {
            _accTorque += torque;
        }

        public float FlushTorque()
        {
            var res = _accTorque;
            _accTorque = 0;
            return res;
        }
    }
}
