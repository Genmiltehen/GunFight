using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace XEngine.Core.Common.Trace
{
    public class TraceSystem : IGameSystem
    {
        private const float Eps = 1e-6f;
        public int Priority => 200;

        public bool IsEnabled { get; set; } = true;

        public void Update(GScene _scene, float _dt)
        {
            foreach (var(e, trace, tr) in _scene.Query<GTrace, GTransform>())
            {
                var pos = tr.RelativePosition2D;
                trace.PointQueue.Enqueue(pos);
                Trim(trace, out var lastPoint);
                if ((lastPoint - pos).Length < Eps && trace.IsAggressive) e.MarkDelete();
            }
        }

        private static void Trim(GTrace trace, out Vector2 last)
        {
            last = Vector2.Zero;
            while (trace.PointQueue.Count > trace.MaxLength) last = trace.PointQueue.Dequeue();
        }
    }
}
