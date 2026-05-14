using OpenTK.Mathematics;
using XEngine.Core.Base;
namespace XEngine.Core.Common.Health
{
    public class GHealthRenderer : GameComponent
    {
        public Vector2 Position { get; private set; }
        public Color4 ForeGround { get; private set; } = Color4.Green;
        public Color4 BackGround { get; private set; } = Color4.Red;

        public GHealthRenderer SetPosition(Vector2 pos)
        {
            Position = pos;
            return this;
        }

        public GHealthRenderer SetColor(Color4? foreground = null, Color4? background = null)
        {
            ForeGround = foreground == null ? ForeGround : foreground.Value;
            BackGround = background == null ? BackGround : background.Value;
            return this;
        }
    }
}
