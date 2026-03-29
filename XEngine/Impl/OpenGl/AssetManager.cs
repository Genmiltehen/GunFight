using GameEngineLib.Impl.RenderImpl;
using GameEngineLib.Impl.SceneImpl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineLib.Impl.OpenGl
{
    public sealed class AssetManager : IAssetLoader, IGLContext, IDisposable
    {
        private readonly string _rootPath;

        public UnitQuad UnitQuad { get; private set; }
        private readonly Dictionary<string, Shader> _shaders = [];

        private readonly Dictionary<string, Texture2D> _persistentTextures = [];
        private readonly Dictionary<string, Texture2D> _sceneTextures = [];

        public AssetManager(string AssetPath)
        {
            _rootPath = Path.Combine(AssetPath);

            UnitQuad = new UnitQuad();
            _shaders["Sprite"] = Shader.FromFiles("sprite.vert", "sprite.frag");

            InitializeErrorShader();
            InitializeDefaultTexture();
        }

        private void InitializeDefaultTexture()
        {
            _persistentTextures["Default_Error"] = Texture2D.FromPath("Textures/Error.png");
        }

        public Shader GetShader(string name)
        {
            if (_shaders.TryGetValue(name, out var shader)) return shader;

            Console.WriteLine($"[Error] Shader '{name}' not found! Falling back to ErrorShader.");
            return _shaders.GetValueOrDefault("Error")
                   ?? throw new Exception("Critical: ErrorShader is missing from AssetManager!");
        }

        public Texture2D LoadTexture(string relativePath, bool isPersistent = false)
        {
            string fullPath = Path.GetFullPath(Path.Combine(_rootPath, relativePath));
            var targetDict = isPersistent ? _persistentTextures : _sceneTextures;

            if (_persistentTextures.TryGetValue(fullPath, out var pTex)) return pTex;
            if (_sceneTextures.TryGetValue(fullPath, out var sTex)) return sTex;

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[Error] Missing texture: {fullPath}");
                return _persistentTextures["Default_Error"];
            }

            var newTex = Texture2D.FromPath(fullPath);
            targetDict.Add(fullPath, newTex);
            return newTex;
        }

        public void UnloadSceneAssets()
        {
            foreach (var tex in _sceneTextures.Values) tex.Dispose();
            _sceneTextures.Clear();
        }

        private void InitializeErrorShader()
        {
            const string vert = @"
        #version 330 core
        layout (location = 0) in vec2 aPos;
        uniform mat4 uModel;
        uniform mat4 uProjection;
        void main() { gl_Position = uProjection * uModel * vec4(aPos, 0.0, 1.0); }";

            const string frag = @"
        #version 330 core
        out vec4 FragColor;
        void main() { FragColor = vec4(1.0, 0.0, 1.0, 1.0); }";

            _shaders["Error"] = Shader.FromSource(vert, frag);
        }

        public void Dispose()
        {
            UnitQuad.Dispose();
            foreach (var _s in _shaders.Values) _s.Dispose();
            _shaders.Clear();
        }
    }
}
