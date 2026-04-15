using System.Diagnostics;
using XEngine.Core.Scenery;

namespace XEngine.Core.Graphics.OpenGL
{
    public sealed class AssetManager : IAssetLoader, IGLContext, IDisposable
    {
        private readonly Dictionary<string, Shader> _shaders = [];
        private readonly Dictionary<string, Texture2D> _persistentTextures = [];
        private readonly Dictionary<string, Texture2D> _sceneTextures = [];

        public readonly string RootPath;
        public string ShaderPath => Path.Combine(RootPath, "Shaders");
        public UnitQuad UnitQuad { get; private set; }


        public AssetManager(string AssetPath)
        {
            RootPath = Path.GetFullPath(AssetPath);
            UnitQuad = new UnitQuad();
        }

        public void Init()
        {
            UnitQuad.Init();
            InitDefaultSpriteShader();
            InitErrorShader();
            InitDefaultTexture();
        }

        private void InitDefaultTexture()
        {
            byte[] pixels = [255, 0, 255, 255, 0, 0, 0, 255, 0, 0, 0, 255, 255, 0, 255, 255];
            _persistentTextures["Default_Error"] = Texture2D.FromBytes(2, 2, pixels);
        }

        private void InitErrorShader()
        {
            var _shaderPath = Path.Combine(RootPath, "Shaders", "Error");
            _shaders["Error"] = Shader.FromFolder(_shaderPath);
        }

        private void InitDefaultSpriteShader()
        {
            var _shaderPath = Path.Combine(RootPath, "Shaders", "Default");
            _shaders["Sprite"] = Shader.FromFolder(_shaderPath);
        }

        public Shader GetShader(string name)
        {
            if (_shaders.TryGetValue(name, out var shader)) return shader;

            Debug.WriteLine($"[Error] Shader '{name}' not found. Falling back to ErrorShader.");
            return _shaders.GetValueOrDefault("Error")
                   ?? throw new Exception("[Error] ErrorShader is missing.");
        }

        public bool SetShader(string name, Shader shader)
        {
            if (_shaders.ContainsKey(name)) return false;
            _shaders[name] = shader;
            return true;
        }

        public Texture2D LoadTexture(string relativePath, bool isPersistent = false)
        {
            string fullPath = Path.GetFullPath(Path.Combine(RootPath, "Textures", relativePath));
            var targetDict = isPersistent ? _persistentTextures : _sceneTextures;

            if (_persistentTextures.TryGetValue(fullPath, out var pTex)) return pTex;
            if (_sceneTextures.TryGetValue(fullPath, out var sTex)) return sTex;

            if (!File.Exists(fullPath))
            {
                Debug.WriteLine($"[Error] Missing texture: {fullPath}");
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

        public void Dispose()
        {
            UnitQuad.Dispose();
            foreach (var _s in _shaders.Values) _s.Dispose();
            _shaders.Clear();
            foreach (var tex in _sceneTextures.Values) tex.Dispose();
            _sceneTextures.Clear();
            foreach (var tex in _persistentTextures.Values) tex.Dispose();
            _persistentTextures.Clear();
        }
    }
}
