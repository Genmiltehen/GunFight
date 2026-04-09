using XEngine.Core.Base;
using XEngine.Core.Scenery;

namespace XEngine.Core.Input
{
    public abstract class InputSystem(InputService input) : IGameSystem
    {
        protected readonly InputService input = input;
        public int Priority => 100;
        public abstract void Update(Scene _scene, float _dt);
    }
}
