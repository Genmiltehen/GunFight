using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using XEngine.Core.Graphics;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace XEngine.Core.Common.Sprite
{
    public class SpriteRendererModule : RenderModule
    {
        public override int Priority => 500;
        private readonly GLProvider _provider;
        private readonly Shader _spriteShader;

        public SpriteRendererModule(GLProvider provider)
        {
            _provider = provider;
            _spriteShader = _provider.GetShader("Sprite");
        }

        public override void Render(GScene scene)
        {
            if (!ValidateDraw(scene)) return;

            _spriteShader.Use();
            _spriteShader.SetMatrix4("uProjection", _projection);

            _spriteShader.SetMatrix4("uView", scene.Camera.GetViewMatrix());
            _spriteShader.SetInt("uTexture", 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            _provider.UnitQuad.Bind();
            foreach (var (_, tr, sprite) in scene.Query<GTransform, GSprite>())
            {
                sprite.Texture.Use();

                var modelMatrix = sprite.GetAssetScale() * tr.GetWorldMatrix();
                _spriteShader.SetMatrix4("uModel", modelMatrix);

                _provider.UnitQuad.Draw();
            }

            GL.BindVertexArray(0);
        }

        private bool ValidateDraw(GScene _scene)
        {
            return !(_screenWidth <= 0 || _screenHeight <= 0);
        }
    }
}
