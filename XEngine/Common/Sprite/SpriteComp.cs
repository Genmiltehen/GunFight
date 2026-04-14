using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Graphics.OpenGL;

namespace XEngine.Core.Common.Sprite
{
    public sealed class SpriteComp : GameComponent
    {
        float Width, Height;
        public SpriteComp Init(Texture2D texture)
        {
            Texture = texture;
            Width = texture.Width;
            Height = texture.Height;
            return this;
        }

        public void SetSize(Vector2 Size)
        {
            Width = Size.X;
            Height = Size.Y;
        }
        public void SetSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public Texture2D Texture { get; set; }
        public Matrix4 GetAssetScale() => Matrix4.CreateScale(Width, Height, 1.0f);
    }
}
