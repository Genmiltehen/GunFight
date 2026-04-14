using OpenTK.Mathematics;
using XEngine.Core.Common;
using XEngine.Core.Physics.Utils;
using XEngine.Core.Utils;

namespace XEngine.Core.Physics.ColliderShapes
{
    public sealed class BoxCollider : ICollider
    {
        private Vector2 _hs;
        private bool _isDirty = true;
        private Box2 _cache_AABB;
        private readonly Vector2[] _cache_points = new Vector2[4];
        private Vector2 _offset;

        public Vector2 Offset
        {
            get => _offset;
            set
            {
                SetDirty();
                _offset = value;
            }
        }
        public Vector2 Size => _hs * 2;
        public ColliderType ColliderType => ColliderType.Box;

        public BoxCollider(Vector2 size)
        {
            _hs = size * 0.5f;
        }

        public Box2 GetBounds(TransformComp tr)
        {
            RecalcInternal(tr);
            return _cache_AABB;
        }

        public Vector2[] GetCorners(TransformComp tr)
        {
            RecalcInternal(tr);
            return _cache_points;
        }

        public void GetProjection(Vector2 axis, TransformComp tr, out float min, out float max)
        {
            Vector2[] corners = GetCorners(tr);
            MathUtils.PolygonBoundsAlongAxis(axis, corners, out min, out max);
        }

        public static void CalcAxes(TransformComp tr, out Vector2 axis1, out Vector2 axis2)
        {
            var (cos, sin) = MathF.SinCos(tr.Rotation);
            axis1 = new Vector2(cos, sin);
            axis2 = new Vector2(-sin, cos);
        }

        public float UnitMassMoI => (Vector2.Dot(Size, Size)) / 12;

        // Optimizations

        public void SetDirty()
        {
            _isDirty = true;
        }

        private void RecalcInternal(TransformComp tr)
        {
            if (_isDirty)
            {
                _isDirty = false;
                RecalcAABB(tr);
                RecalcCorners(tr);
            }
        }

        private void RecalcCorners(TransformComp tr)
        {
            var wm = tr.GetWorldMatrix();
            _cache_points[0] = (new Vector4(Offset.X + _hs.X, Offset.Y + _hs.Y, 0, 1) * wm).Xy;
            _cache_points[1] = (new Vector4(Offset.X + _hs.X, Offset.Y - _hs.Y, 0, 1) * wm).Xy;
            _cache_points[2] = (new Vector4(Offset.X - _hs.X, Offset.Y - _hs.Y, 0, 1) * wm).Xy;
            _cache_points[3] = (new Vector4(Offset.X - _hs.X, Offset.Y + _hs.Y, 0, 1) * wm).Xy;
        }

        private void RecalcAABB(TransformComp tr)
        {
            var rot = tr.Rotation;
            Vector2 center = tr.Position2D + Offset;

            if (rot == 0) _cache_AABB = new Box2(center - _hs, center + _hs);

            float pcos = MathF.Abs(MathF.Cos(rot));
            float psin = MathF.Abs(MathF.Sin(rot));
            Vector2 QSize = new(
                _hs.X * pcos + _hs.Y * psin,
                _hs.X * psin + _hs.Y * pcos
            );
            _cache_AABB = new Box2(center - QSize, center + QSize);
        }
    }
}
