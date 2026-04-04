using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Defaults;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game
{
    public class TestSystem : IGameSystem
    {
        private float _t;
        public TestSystem()
        {
            _t = 0;
        }

        public int Priority => 0;

        public void Update(Scene _scene, float _dt)
        {
            float x = 100;
            _t += _dt;
            foreach (var (_e, transform) in _scene.Query<TransformComp>(_e => _e.Has<PlayerTag>()))
            {
                transform.Position = new Vector3((float)Math.Cos(_t), (float)Math.Sin(_t), transform.Position.Z / x) * x;
                transform.Scale = new Vector2((float)Math.Cos(_t) + 2, (float)Math.Cos(_t) + 2) / 3;
                transform.Rotation = _t;
            }
        }
    }
}
