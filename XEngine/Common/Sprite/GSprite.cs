using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Graphics.OpenGL;

namespace XEngine.Core.Common.Sprite
{
    public sealed class GSprite : GameComponent
    {
        public Texture2D Texture { get; private set; }
        private Matrix4 _translation = Matrix4.Identity;
        private Matrix4 _rotation = Matrix4.Identity;
        private Matrix4 _scale = Matrix4.Identity;
        private Matrix4 _tr = Matrix4.Identity;
        private bool _useTextureScale = false;
        private float _textureScale = 1;

        public GSprite SetTexture(Texture2D texture, bool _useTextureScale)
        {
            Texture = texture;
            this._useTextureScale = _useTextureScale;
            Recalculate();
            return this;
        }

        public GSprite SetTranslation(Vector2 vec)
        {
            _translation = Matrix4.CreateTranslation(vec.X, vec.Y, 0);
            Recalculate();
            return this;
        }

        public GSprite SetRotation(float rotation)
        {
            _rotation = Matrix4.CreateRotationZ(rotation);
            Recalculate();
            return this;
        }

        public GSprite SetScale(Vector2 scale)
        {
            _scale = Matrix4.CreateScale(scale.X, scale.Y, 1);
            Recalculate();
            return this;
        }

        public GSprite SetSourceTextureScale(float scale)
        {
            _textureScale = scale;
            return this;
        }

        private void Recalculate()
        {
            var _texScale = _useTextureScale && Texture != null
                ? Matrix4.CreateScale(Texture.Width * _textureScale, Texture.Height * _textureScale, 1)
                : Matrix4.Identity;
            _tr = _texScale * _scale * _rotation * _translation;
        }

        public Matrix4 GetAssetScale() => _tr;
    }
}
