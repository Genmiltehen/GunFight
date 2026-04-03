using GameEngineLib.Impl;
using GameEngineLib.Impl.OpenGl;
using OpenTK.Mathematics;

namespace GameEngineLib.Defaults.Render
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
