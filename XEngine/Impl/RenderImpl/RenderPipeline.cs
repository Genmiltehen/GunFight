using GameEngineLib.Impl.SceneImpl;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineLib.Impl.RenderImpl
{
    public class RenderPipeline
    {
        private readonly List<IRenderSystem> _renderers = [];
        private readonly List<CameraComp> _activeCameras = [];

        public void AddRenderer(IRenderSystem renderer)
        {
            _renderers.Add(renderer);
            _renderers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void RegisterCamera(CameraComp camera)
        {
            if (!_activeCameras.Contains(camera))
                _activeCameras.Add(camera);
            _activeCameras.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public void UnregisterCamera(CameraComp camera) => _activeCameras.Remove(camera);

        public void SetViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            foreach (var _r in _renderers) _r.OnResize(width, height);
        }

        public void Render(Scene scene)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (_activeCameras.Count == 0) return;

            foreach (var camera in _activeCameras)
                foreach (var _r in _renderers)
                    if (_r.IsEnabled)
                        _r.Render(scene, camera);
        }
    }
}
