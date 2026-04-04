using XEngine.Core.Scenery;

namespace XEngine.Core.Base
{
    public interface IGameSystem
    {
        public int Priority { get; }
        void Update (Scene _scene, float _dt);
    }
}
