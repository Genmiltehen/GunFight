using OpenTK.Mathematics;
using XEngine.Core.Physics.Collision;
using XEngine.Core.Utils;

namespace XEngine.Core.Physics.MathStructs
{
    public struct CollisionManifold
    {
        public CollisionObject coA;
        public CollisionObject coB;

        public Vector2 Normal; // A -> B
        public ContactPoint contact1;
        public ContactPoint contact2;
        public int contactCount;

        public readonly float CalculateK_inv(ContactPoint PoC, Vector2 Direction, out float r_APerpN, out float r_BPerpN)
        {
            Vector2 r_A = PoC.point - coA.tr.Position2D;
            Vector2 r_B = PoC.point - coB.tr.Position2D;
            r_APerpN = MathUtils.Cross2D(r_A, Direction);
            r_BPerpN = MathUtils.Cross2D(r_B, Direction);
            return coA.rb.InvMass + coB.rb.InvMass + 
                r_APerpN * r_APerpN * coA.rb.InvInertia + 
                r_BPerpN * r_BPerpN * coB.rb.InvInertia;
        }
    }
}
