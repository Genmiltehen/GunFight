using OpenTK.Mathematics;

namespace XEngine.Core.Utils
{
    public readonly struct Segment
    {
        public readonly Vector2 start;
        public readonly  Vector2 end;

        public Segment(Vector2 start, Vector2 end) : this()
        {
            this.start = start;
            this.end = end;
            _l = (end - start).Length;
            _dir = (end - start).Normalized();
        }

        private readonly float _l;    
        private readonly Vector2 _dir;

        public readonly float Length => _l;
        public readonly Vector2 Direction => _dir;
        public readonly Vector2 Vector => end - start;
        public readonly Vector2 MidPoint => (start + end) / 2;

        public readonly Vector2[] AsArray() => [start, end];

        public readonly Vector2 Lerp(float t)
        {
            return Vector2.Lerp(start, end, t);
        }

        public readonly Vector2 ClampLerp(float t)
        {
            return Vector2.Lerp(start, end, Math.Clamp(t, 0, 1));
        }
    }
}
