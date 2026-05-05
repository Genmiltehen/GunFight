using OpenTK.Mathematics;
using XEngine.Core.Base;

namespace XEngine.Core.Common.Trace
{
    public class GTrace : GameComponent
    {
        public readonly Queue<Vector2> PointQueue = new();
        public Vector3 Color;
        public int MaxLength;
        public bool IsAggressive;

        public GTrace Init(Vector3 color, int length)
        {
            Color = color;
            MaxLength = length;
            return this;
        }
    }
}
