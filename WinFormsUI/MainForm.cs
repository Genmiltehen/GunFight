using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using WinFormsUI.Game;
using WinFormsUI.Game.Input;
using XEngine.Core;
using XEngine.Core.Graphics.OpenGL;

namespace WinFormsUI
{
    public partial class MainForm : Form
    {
        private readonly GameEngine _engine;

        private readonly Stopwatch _stopwatch = new();

        private double _accumulator = 0;
        private double _lastTime = 0;
        private const double TargetUpdateFPS = 60.0;
        private const double _dt = 1.0 / TargetUpdateFPS;

        public MainForm()
        {
            InitializeComponent();

            string assetsPath = Path.Combine("D:\\!!GSTU\\C2\\S2\\!Kurs\\GunFight", "Assets");
            _engine = new GameEngine(assetsPath);
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            Debug.WriteLine("formload");
            int width = ClientSize.Width;
            int height = ClientSize.Height;
            _engine.Renderer.SetViewport(width, height);

            _engine.SceneManager.SwitchTo(new TestScene(_engine));

            _stopwatch.Start();
            MainTimer.Start();

            Activate();
        }

        private void OnGLLoad(object sender, EventArgs e)
        {
            Debug.WriteLine("glload");
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.ClearColor(Color.Black);

            _engine.Init();

            var debugShaderFolder = Path.Combine(_engine.Assets.ShaderPath, "Debuging");
            _engine.Assets.SetShader("CapsuleDebug", Shader.FromFolder(Path.Combine(debugShaderFolder, "Capsule")));
            _engine.Assets.SetShader("BoxDebug", Shader.FromFolder(Path.Combine(debugShaderFolder, "Box")));
        }

        private void OnGLPaint(object sender, PaintEventArgs e)
        {
            MainGLControl.MakeCurrent();
            _engine.Render();
            MainGLControl.SwapBuffers();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            double currentTime = _stopwatch.Elapsed.TotalSeconds;
            double frameTime = currentTime - _lastTime;
            _lastTime = currentTime;
            _accumulator += frameTime;

            while (_accumulator >= _dt)
            {
                _engine.Update((float)_dt);
                _accumulator -= _dt;
            }

            MainGLControl.Invalidate();
        }

        private void MainFormResize(object sender, EventArgs e)
        {
            int width = ClientSize.Width;
            int height = ClientSize.Height;

            if (width > 0 && height > 0) _engine.Renderer.SetViewport(width, height);
        }

        private void OnClose(object sender, FormClosingEventArgs e)
        {
            _engine.Assets.Dispose();
        }

        private void MainForm_OnKeyDown(object sender, KeyEventArgs e) => _engine.Input.SetKeyDown(KeyConverter.ToOpenTK(e.KeyCode));
        private void MainForm_OnKeyUp(object sender, KeyEventArgs e) => _engine.Input.SetKeyUp(KeyConverter.ToOpenTK(e.KeyCode));
        private void OnLostGlobalFacus(object sender, EventArgs e) => _engine.Input.ClearStates();
    }
}
