using GameEngineLib.Impl;
using GameEngineLib.Impl.OpenGl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace GameEngineLib.Defaults
{
    public sealed class SpriteComp(Texture2D texture) : IGameComponent
    {
        public Texture2D Texture = texture;

        public Matrix4 GetAssetScale() => Matrix4.CreateScale(Texture.Width, Texture.Height, 1.0f);
    }
}
