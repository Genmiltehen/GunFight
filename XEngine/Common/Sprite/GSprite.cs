using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Graphics.OpenGL;

namespace XEngine.Core.Common.Sprite
{
    public class GSprite : GameComponent
    {
        public Texture2D Texture { get; set; } = null!;

        protected Vector2 _position = Vector2.Zero;
        protected float _rotation = 0f;
        protected Vector2 _size = Vector2.One;

        protected Matrix4 _modelMatrix = Matrix4.Identity;
        protected bool _isDirty = true;
        public bool IsUseTextureSize { get; protected set; } = false;
        public bool FlipX = false;
        public bool FlipY = false;

        public GSprite SetTexture(Texture2D texture, bool useTextureSize)
        {
            Texture = texture;
            IsUseTextureSize = useTextureSize;
            _isDirty = true;
            return this;
        }

        public GSprite SetTranslation(Vector2 vec)
        {
            _position = vec;
            _isDirty = true;
            return this;
        }

        public GSprite SetRotation(float rotation)
        {
            _rotation = rotation;
            _isDirty = true;
            return this;
        }

        public GSprite SetSize(Vector2 size)
        {
            _size = size;
            _isDirty = true;
            return this;
        }

        public Matrix4 GetModelMatrix()
        {
            if (_isDirty) Recalculate();
            return _modelMatrix;
        }

        protected void Recalculate()
        {
            if (Texture == null)
            {
                _modelMatrix = Matrix4.Identity;
                _isDirty = false;
                return;
            }

            var scaleMat = Matrix4.CreateScale(_size.X, _size.Y, 1f);
            var rotMat = Matrix4.CreateRotationZ(_rotation);
            var transMat = Matrix4.CreateTranslation(_position.X, _position.Y, 0f);

            _modelMatrix = scaleMat * rotMat * transMat;
            _isDirty = false;
        }

        public void UseTexture() => Texture.Use();

        public Vector2 TextureSize => new(Texture.Width, Texture.Height);
    }
}
