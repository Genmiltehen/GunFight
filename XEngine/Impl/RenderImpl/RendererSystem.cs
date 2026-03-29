using GameEngineLib.Defaults;
using GameEngineLib.Impl.OpenGl;
using GameEngineLib.Impl.SceneImpl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngineLib.Impl.RenderImpl
{
    public class RendererSystem : IGameSystem
    {
        private readonly Camera _camera;
        private readonly IGLContext _context;
        public int Priority => 500;

        private int _screenWidth;
        private int _screenHeight;

        public RendererSystem(IGLContext context, Camera camera)
        {
            _camera = camera;
            _context = context;
        }

        public void SetViewport(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
            GL.Viewport(0, 0, width, height);
        }

        public void Update(Scene _scene, float _dt)
        {
            if (!ValidateDraw(_scene)) return;
            var _shader = _context.GetShader("Sprite");
            var _quadMesh = _context.UnitQuad;

            _shader.Use();

            float halfW = _screenWidth / 2f, halfH = _screenHeight / 2f;
            var projection = Matrix4.CreateOrthographicOffCenter(-halfW, halfW, -halfH, halfH, -1f, 1f);
            _shader.SetMatrix4("uProjection", projection);

            var view = _camera.GetViewMatrix();
            _shader.SetMatrix4("uView", view);

            _quadMesh.Bind();
            foreach (var (_, transform, sprite) in _scene.Query<TransformComp, SpriteComp>())
            {
                GL.ActiveTexture(TextureUnit.Texture0);

                sprite.Texture.Use();
                _shader.SetInt("uTexture", 0);

                var modelMatrix = sprite.GetAssetScale() * transform.GetMatrix();
                _shader.SetMatrix4("uModel", modelMatrix);

                _quadMesh.Draw();
            }

            GL.BindVertexArray(0);
        }

        private bool ValidateDraw(Scene _scene)
        {
            return !(_screenWidth <= 0 || _screenHeight <= 0);
        }
    }
}
