using OpenTK.Mathematics;
using System.Xml.Linq;
using XEngine.Core.Base;
using XEngine.Core.Physics.Utils;

namespace XEngine.Core.Common
{
    public sealed class TransformComp : GameComponent, IDirty
    {
        private Vector3 _position = Vector3.Zero;
        private float _rotation = 0;
        private Vector2 _scale = Vector2.One;

        private Matrix4 _cachedMatrix;
        private bool _isDirty = true;

        public TransformComp? Parent { get; private set; } = null;
        public TransformComp? FirstChild { get; private set; } = null;
        public TransformComp? NextSibling { get; private set; } = null;
        public TransformComp? PrevSibling { get; private set; } = null;

        public TransformComp Init(Vector3 pos, float rotation, Vector2 scale)
        {
            _position = pos;
            _rotation = rotation;
            _scale = scale;
            return this;
        }

        public void SetParent(TransformComp? newParent)
        {
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

        public TransformComp? GetChild(int index)
        {
            TransformComp? current = FirstChild;
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
            Owner.Get<ColliderComp>()?.SetDirty();
            if (_isDirty) return;
            _isDirty = true;

            TransformComp? current = FirstChild;
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
            set { if (_position != value) { _position = value; SetDirty(); } }
        }

        public float Rotation
        {
            get => _rotation;
            set { if (_rotation != value) { _rotation = value; SetDirty(); } }
        }

        public float RotationDegrees
        {
            get => MathHelper.RadiansToDegrees(_rotation);
            set => Rotation = MathHelper.DegreesToRadians(value);
        }

        public Vector2 Scale
        {
            get => _scale;
            set { if (_scale != value) { _scale = value; SetDirty(); } }
        }

        public Matrix4 GetWorldMatrix()
        {
            if (_isDirty)
            {
                //Matrix4 localMatrix = Matrix4.CreateScale(_scale.X, _scale.Y, 1.0f) *
                //             Matrix4.CreateRotationZ(_rotation) *
                //             Matrix4.CreateTranslation(_position);

                //if (Parent != null) _cachedMatrix = localMatrix * Parent.GetWorldMatrix();
                //else _cachedMatrix = localMatrix;

                _cachedMatrix = Matrix4.CreateScale(_scale.X, _scale.Y, 1.0f) *
                                Matrix4.CreateRotationZ(_rotation) *
                                Matrix4.CreateTranslation(_position);

                if (Parent != null) _cachedMatrix *= Parent.GetWorldMatrix();

                _isDirty = false;
            }
            return _cachedMatrix;
        }
    }
}
