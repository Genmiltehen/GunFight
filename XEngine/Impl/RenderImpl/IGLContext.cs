using GameEngineLib.Impl.OpenGl;

namespace GameEngineLib.Impl.RenderImpl
{
    public interface IGLContext
    {
        UnitQuad UnitQuad { get; }
        Shader GetShader(string name);
    }
}
