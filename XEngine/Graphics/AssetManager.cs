using System.Diagnostics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace XEngine.Core.Graphics
{
    public sealed class AssetManager : IAssetLoader, IDisposable
    {
        private readonly Dictionary<string, Texture2D> _persistentTextures = [];
        private readonly Dictionary<string, Texture2D> _sceneTextures = [];
        private bool _disposed = false;

        public readonly string RootPath;
        public string ShaderPath => Path.Combine(RootPath, "Shaders");


        public AssetManager(string AssetPath)
        {
            RootPath = Path.GetFullPath(AssetPath);
        }

        public void Init()
        {
            InitDefaultTexture();
        }

        private void InitDefaultTexture()
        {
            byte[] pixels = [255, 0, 255, 255, 0, 0, 0, 255, 0, 0, 0, 255, 255, 0, 255, 255];
            _persistentTextures["Default_Error"] = Texture2D.FromBytes(2, 2, pixels);
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
            if (_disposed) return;
            _disposed = true;

            foreach (var tex in _sceneTextures.Values) tex.Dispose();
            _sceneTextures.Clear();
            foreach (var tex in _persistentTextures.Values) tex.Dispose();
            _persistentTextures.Clear();
        }
    }
}
