using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace XEngine.Core.Common.LifeTime
{
    public class LifeTimerSystem : IGameSystem
    {

        public int Priority => 200;

        public bool IsEnabled { get; set; } = true;

        public void Update(GScene _scene, float _dt)
        {
            foreach (var (e, _) in _scene.Query<GLifeTime>((_, t) => t.LifeTimer.IsFinished)) e.MarkDelete();
        }
    }
}
