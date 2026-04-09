using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Graphics.OpenGL;

namespace XEngine.Core.Common.Sprite
{
    public sealed class SpriteComp : GameComponent
    {
        public SpriteComp Init(Texture2D texture)
        {
            Texture = texture;
            return this;
        }

        public Texture2D Texture { get; set; }
        public Matrix4 GetAssetScale() => Matrix4.CreateScale(Texture.Width, Texture.Height, 1.0f);
    }
}
