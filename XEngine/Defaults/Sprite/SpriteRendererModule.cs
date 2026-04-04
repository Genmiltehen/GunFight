using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using XEngine.Core.Graphics;
using XEngine.Core.Scenery;

namespace XEngine.Core.Defaults.Sprite
{
    public class SpriteRendererModule : IRenderModule
    {
        public int Priority => 500;
        public bool IsEnabled { get; set; } = true;

        private readonly IGLContext _context;
        private int _screenWidth;
        private int _screenHeight;
        private Matrix4 _projection;

        public SpriteRendererModule(IGLContext context)
        {
            _context = context;
        }

        public void OnResize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
            float halfW = _screenWidth / 2f;
            float halfH = _screenHeight / 2f;
            _projection = Matrix4.CreateOrthographicOffCenter(-halfW, halfW, -halfH, halfH, -1f, 1f);
        }

        public void Render(Scene scene, CameraComp camera)
        {
            if (!ValidateDraw(scene)) return;

            var shader = _context.GetShader("Sprite");
            shader.Use();

            shader.SetMatrix4("uProjection", _projection);
            shader.SetMatrix4("uView", camera.GetViewMatrix());
            shader.SetInt("uTexture", 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            _context.UnitQuad.Bind();
            foreach (var (_, transform, sprite) in scene.Query<TransformComp, SpriteComp>())
            {
                sprite.Texture.Use();

                var modelMatrix = sprite.GetAssetScale() * transform.GetMatrix();
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
