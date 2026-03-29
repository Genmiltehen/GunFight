using GameEngineLib.Impl.OpenGl;

namespace GameEngineLib.Impl.SceneImpl
{
    public interface IAssetLoader
    {
        Texture2D LoadTexture(string path, bool persistent = false);
    }
}
