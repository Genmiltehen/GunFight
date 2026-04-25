using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.Security.Policy;
using XEngine.Core.Graphics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace XEngine.Core.Common.Sprite.NineSlice
{
    public class NineSliceRendererModule : RenderModule
    {
        public override int Priority => 500;
        private readonly GLProvider _provider;
        private readonly Shader _nineShader;

        public NineSliceRendererModule(GLProvider provider)
        {
            _provider = provider;
            _nineShader = _provider.GetShader("NineSlice");
        }

        public override void Render(GScene scene)
        {
            if (_screenSize.X <= 0 || _screenSize.Y <= 0) return;

            _nineShader.Use();

            _nineShader.SetMatrix4("uProjection", scene.Camera.GetProjectionMatrix(_screenSize));
            _nineShader.SetMatrix4("uView", scene.Camera.GetViewMatrix());

            _nineShader.SetInt("uTexture", 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            _provider.UnitQuad.Bind();
            foreach (var (_, tr, nineslice) in scene.Query<GTransform, GNineSlice>())
            {
                _nineShader.SetVector2("uTexSize", nineslice.TextureSize);
                _nineShader.SetVector4("uBorder", nineslice.Borders);
                _nineShader.SetVector2("uRenderSize", nineslice.RenderSize);
                nineslice.UseTexture();

                var modelMatrix = scene.World.PPMScale * nineslice.GetModelMatrix() * tr.GetWorldMatrix();
                _nineShader.SetMatrix4("uModel", modelMatrix);

                _provider.UnitQuad.Draw();
            }

            GL.BindVertexArray(0);
        }
    }
}
