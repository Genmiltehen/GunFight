using XEngine.Core.Graphics.OpenGL;

namespace XEngine.Core.Scenery
{
    public interface IAssetLoader
    {
        Texture2D LoadTexture(string path, bool persistent = false);
    }
}
