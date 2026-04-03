using GameEngineLib.Impl.SceneImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineLib.Impl.RenderImpl
{
    public class RenderPipeline
    {
        private readonly List<IRenderSystem> _renderers = [];

        public void AddRenderer(IRenderSystem renderer)
        {
            _renderers.Add(renderer);
            _renderers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void Render(Scene scene, Camera camera)
        {
            foreach (var _r in _renderers) if (_r.IsEnabled) _r.Render(scene, camera);
        }
    }
}
