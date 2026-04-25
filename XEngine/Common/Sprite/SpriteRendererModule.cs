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
            if (_screenSize.X <= 0 || _screenSize.Y <= 0) return;

            _spriteShader.Use();

            _spriteShader.SetMatrix4("uProjection", scene.Camera.GetProjectionMatrix(_screenSize));
            _spriteShader.SetMatrix4("uView", scene.Camera.GetViewMatrix());
            _spriteShader.SetInt("uTexture", 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            _provider.UnitQuad.Bind();
            foreach (var (_, tr, sprite) in scene.Query<GTransform, GSprite>())
            {
                sprite.UseTexture();

                var ts = sprite.TextureSize;
                var texSize = sprite.IsUseTextureSize ? Matrix4.CreateScale(ts.X, ts.Y, 1) : Matrix4.Identity;
                var modelMatrix = scene.World.PPMScale * texSize * sprite.GetModelMatrix() * tr.GetWorldMatrix();
                _spriteShader.SetMatrix4("uModel", modelMatrix);

                _provider.UnitQuad.Draw();
            }

            GL.BindVertexArray(0);
        }
    }
}
