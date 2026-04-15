using OpenTK.Mathematics;

namespace XEngine.Core.Utils
{
    public struct Segment
    {
        public Vector2 start;
        public Vector2 end;

        public readonly float Length => Vector.Length;
        public readonly Vector2 Vector => end - start;
        public readonly Vector2 Direction => Vector.Normalized();
        public readonly Vector2 MidPoint => (start + end) / 2;

        public readonly Vector2[] AsArray() => [start, end];

        public readonly Vector2 Lerp(float t)
        {
            return Vector2.Lerp(start, end, t);
        }
    }
}
