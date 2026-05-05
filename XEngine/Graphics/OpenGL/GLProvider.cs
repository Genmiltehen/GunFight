using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEngine.Core.Graphics.OpenGL
{
    public sealed class GLProvider(string shaderFolder) : IDisposable
    {
        private readonly Dictionary<string, Shader> _shaders = [];
        private bool _disposed = false;

        public readonly string ShaderFolder = shaderFolder;
        public UnitQuad UnitQuad { get; private set; } = new UnitQuad();
        public LineBatcher LineBatcher { get; private set; } = new LineBatcher();

        public void Init()
        {
            UnitQuad.Init();
            LineBatcher.Init();
            InitDefaultSpriteShader();
            InitErrorShader();
        }

        public Shader GetShader(string name)
        {
            if (_shaders.TryGetValue(name, out var shader)) return shader;

            Debug.WriteLine($"[Error] Shader '{name}' not found. Falling back to ErrorShader.");
            return _shaders.GetValueOrDefault("Error")
                   ?? throw new Exception("[Fatal] ErrorShader is missing.");
        }

        /// <summary>
        /// Uses Shader.FromFolder
        /// </summary>
        /// <param name="path">Path in Assets/Shaders/</param>
        /// <param name="name">Internal shader name to set</param>
        /// <returns></returns>
        public Shader LoadShader(string path, string name)
        {
            string _shader_path = Path.Combine(ShaderFolder, path);
            var res = Shader.FromFolder(_shader_path);
            SetShader(name, res);
            return res;
        }

        private bool SetShader(string name, Shader shader)
        {
            if (_shaders.ContainsKey(name)) return false;
            _shaders[name] = shader;
            return true;
        }

        private void InitErrorShader()
        {
            var _shaderPath = Path.Combine(ShaderFolder, "Error");
            _shaders["Error"] = Shader.FromFolder(_shaderPath);
        }

        private void InitDefaultSpriteShader()
        {
            var _shaderPath = Path.Combine(ShaderFolder, "Default");
            _shaders["Sprite"] = Shader.FromFolder(_shaderPath);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var _s in _shaders.Values) _s.Dispose();
            _shaders.Clear();
        }
    }
}
