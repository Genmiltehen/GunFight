using GameEngineLib.Impl;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineLib.Defaults
{
    public class TransformComp : IGameComponent
    {
        private Vector3 _position;
        private float _rotation;
        private Vector2 _scale = Vector2.One;

        private Matrix4 _cachedMatrix;
        private bool _isDirty = true;

        public Vector3 Position
        {
            get => _position;
            set { if (_position != value) { _position = value; _isDirty = true; } }
        }

        public float Rotation
        {
            get => _rotation;
            set { if (_rotation != value) { _rotation = value; _isDirty = true; } }
        }

        public float RotationDegrees
        {
            get => MathHelper.RadiansToDegrees(_rotation);
            set => Rotation = MathHelper.DegreesToRadians(value);
        }

        public Vector2 Scale
        {
            get => _scale;
            set { if (_scale != value) { _scale = value; _isDirty = true; } }
        }

        public Matrix4 GetMatrix()
        {
            if (_isDirty)
            {
                _cachedMatrix = Matrix4.CreateScale(_scale.X, _scale.Y, 1.0f) *
                                Matrix4.CreateRotationZ(_rotation) *
                                Matrix4.CreateTranslation(_position);

                _isDirty = false;
            }
            return _cachedMatrix;
        }
    }
}
