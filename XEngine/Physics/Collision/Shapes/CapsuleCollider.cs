using OpenTK.Mathematics;
using XEngine.Core.Common;
using XEngine.Core.Utils;

namespace XEngine.Core.Physics.Collision.Shapes
{
    public class CapsuleCollider : ICollider
    {
        private readonly float _hl;
        private readonly float _r;
        private Segment _cache_seg;
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
            GetSegment(tr, out Segment seg);
            return new Box2(seg.start, seg.end).Inflated(new Vector2(_r, _r));
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

        public void GetSegment(TransformComp tr, out Segment segment)
        {
            if (_isDirty)
            {
                Vector2 center = tr.Position2D + Offset;
                var (sin, cos) = MathF.SinCos(tr.Rotation);
                Vector2 dir = new(cos, sin);
                _cache_seg = new()
                {
                    start = center - dir * _hl,
                    end = center + dir * _hl
                };
                _isDirty = false;
            }
            segment = _cache_seg;
        }

        public void SetDirty()
        {
            _isDirty = true;
        }
    }
}
