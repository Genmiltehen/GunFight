using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Input;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game
{
    public class TestSystem(InputService input) : InputSystem(input)
    {
        public override void Update(Scene _scene, float _dt)
        {
            float h = input.GetAxis("Horzontal");
            float v = input.GetAxis("Vertical");
            foreach (var (_e, transform) in _scene.Query<TransformComp>(_e => _e.Has<PlayerTag>()))
            {
                transform.Position += new Vector3(h, v, 0) * _dt * 100;
            }
        }
    }
}
