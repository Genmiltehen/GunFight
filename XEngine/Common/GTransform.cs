using Box2D.NET;
using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;

namespace XEngine.Core.Common
{
    public sealed class GTransform : GameComponent, IDirtiable
    {
        private Vector3 _position = Vector3.Zero;
        private float _rotation = 0;
        private Matrix4 _cachedMatrix;
        private bool _isDirty = true;

        public GTransform? Parent { get; private set; } = null;
        public GTransform? FirstChild { get; private set; } = null;
        public GTransform? NextSibling { get; private set; } = null;
        public GTransform? PrevSibling { get; private set; } = null;

        public bool IsSynced { get; private set; } = true;

        public GTransform Init(Vector3 pos, float rotation)
        {
            _position = pos;
            _rotation = rotation;
            IsSynced = false;
            return this;
        }

        public void SetParent(GTransform? newParent)
        {
            if (Owner.Get<GBox2DBody>() is not null)
            {
                throw new InvalidOperationException("Cant link entity's transform with Box2DBody, use Box2D constraint");
            }

            if (Parent != null)
            {
                if (Parent.FirstChild == this) Parent.FirstChild = NextSibling;
                if (PrevSibling != null) PrevSibling.NextSibling = NextSibling;
                if (NextSibling != null) NextSibling.PrevSibling = PrevSibling;
            }
            Parent = newParent;
            if (Parent != null)
            {
                NextSibling = Parent.FirstChild;
                if (NextSibling != null) NextSibling.PrevSibling = this;
                Parent.FirstChild = this;
                PrevSibling = null;
            }
        }

        public GTransform? GetChild(int index)
        {
            GTransform? current = FirstChild;
            int count = 0;
            while (current != null)
            {
                if (count == index) return current;
                count++;
                current = current.NextSibling;
            }
            throw new System.IndexOutOfRangeException();
        }

        public void SetDirty()
        {
            if (_isDirty) return;
            _isDirty = true;

            GTransform? current = FirstChild;
            while (current != null)
            {
                current.SetDirty();
                current = current.NextSibling;
            }
        }

        public Vector2 Position2D
        {
            get => new(_position.X, _position.Y);
            set => Position = new Vector3(value.X, value.Y, _position.Z);
        }

        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    SetDirty();
                    IsSynced = false;
                }
            }
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    SetDirty();
                    IsSynced = false;
                }
            }
        }

        public void SyncToBody(GBox2DBody gbody)
        {
            var pos = B2Bodies.b2Body_GetPosition(gbody.Id);
            var rot = B2MathFunction.b2Rot_GetAngle(B2Bodies.b2Body_GetRotation(gbody.Id));
            _position.X = pos.X;
            _position.Y = pos.Y;
            _rotation = rot;
            SetDirty();
        }

        public Matrix4 GetWorldMatrix()
        {
            if (_isDirty)
            {
                _cachedMatrix = Matrix4.CreateRotationZ(_rotation) *
                                Matrix4.CreateTranslation(_position);
                if (Parent != null) _cachedMatrix *= Parent.GetWorldMatrix();
                _isDirty = false;
            }
            return _cachedMatrix;
        }
    }
}
