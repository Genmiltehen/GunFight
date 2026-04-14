using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using XEngine.Core.Graphics;
using XEngine.Core.Scenery;

namespace XEngine.Core.Common.Sprite
{
    public class SpriteRendererModule : RenderModule
    {
        public override int Priority => 500;
        private readonly IGLContext _context;

        public SpriteRendererModule(IGLContext context)
        {
            _context = context;
        }

        public override void Render(Scene scene)
        {
            if (!ValidateDraw(scene)) return;

            var shader = _context.GetShader("Sprite");
            shader.Use();

            shader.SetMatrix4("uProjection", _projection);
            shader.SetMatrix4("uView", scene.Camera.GetViewMatrix());
            shader.SetInt("uTexture", 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            _context.UnitQuad.Bind();
            foreach (var (_, transform, sprite) in scene.Query<TransformComp, SpriteComp>())
            {
                sprite.Texture.Use();

                var modelMatrix = sprite.GetAssetScale() * transform.GetWorldMatrix();
                shader.SetMatrix4("uModel", modelMatrix);

                _context.UnitQuad.Draw();
            }

            GL.BindVertexArray(0);
        }

        private bool ValidateDraw(Scene _scene)
        {
            return !(_screenWidth <= 0 || _screenHeight <= 0);
        }
    }
}
