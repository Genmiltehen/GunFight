using XEngine.Core.Graphics.OpenGL;

namespace XEngine.Core.Graphics
{
    public interface IGLContext
    {
        UnitQuad UnitQuad { get; }
        Shader GetShader(string name);
    }
}
