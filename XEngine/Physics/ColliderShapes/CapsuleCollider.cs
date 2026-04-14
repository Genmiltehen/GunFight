using OpenTK.Mathematics;
using System.Drawing;
using XEngine.Core.Common;
using XEngine.Core.Physics.Utils;

namespace XEngine.Core.Physics.ColliderShapes
{
    public class CapsuleCollider : ICollider
    {
        private readonly float _hl;
        private readonly float _r;
        private Vector2 _cache_a;
        private Vector2 _cache_b;
        private bool _isDirty = true;
        private Vector2 _offset;

        public ColliderType ColliderType => ColliderType.Capsule;
        public Vector2 Offset
        {
            get => _offset;
            set
            {
                SetDirty();
                _offset = value;
            }
        }
        public float Length => _hl * 2;
        public float Radius => _r;


        public CapsuleCollider(float length, float radius)
        {
            _hl = length / 2;
            _r = radius;
        }

        public Box2 GetBounds(TransformComp tr)
        {
            GetPoints(tr, out Vector2 a, out Vector2 b);
            return new Box2(a, b).Inflated(new Vector2(_r, _r));
        }

        public float UnitMassMoI
        {
            get
            {
                float A_rect = 2 * Radius * Length;
                float A_circ = MathF.PI * Radius * Radius;
                float A_total = A_rect + A_circ;
                float m_rect = A_rect / A_total;
                float m_circ = A_circ / A_total;

                float MoI_rect = m_rect * (4 * Radius * Radius + Length * Length) / 12;
                float MoI_cisc = m_circ * (Radius * Radius / 2 + _hl * _hl);

                return MoI_rect + MoI_cisc;
            }
        }

        public void GetPoints(TransformComp tr, out Vector2 a, out Vector2 b)
        {
            if (_isDirty)
            {
                Vector2 center = tr.Position2D + Offset;
                var (sin, cos) = MathF.SinCos(tr.Rotation);
                Vector2 dir = new(cos, sin);
                _cache_a = center - dir * _hl;
                _cache_b = center + dir * _hl;
                _isDirty = false;
            }
            a = _cache_a; b = _cache_b;
        }

        public void SetDirty()
        {
            _isDirty = true;
        }
    }
}
