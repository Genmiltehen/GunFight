using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using WinFormsUI.Game.Input;
using WinFormsUI.Game.Player;
using WinFormsUI.Game.Scenes;
using XEngine.Core;
using XEngine.Core.Common.Health;
using XEngine.Core.Common.Trace;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace WinFormsUI
{
    public partial class MainForm : Form
    {
        private readonly GameEngine _engine;
        private MainScene _scene = null!;

        private readonly Stopwatch _stopwatch = new();

        private double _accumulator = 0;
        private double _lastTime = 0;
        private const double TargetUpdateFPS = 60.0;
        private const double _dt = 1.0 / TargetUpdateFPS;

        public MainForm()
        {
            InitializeComponent();
            _engine = new GameEngine();
            KeyPreview = true;
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            _engine.Renderer.AddRenderModule(new TracerRenderModule(_engine.GLProvider));
            _engine.Renderer.AddRenderModule(new BasicHealthRenderModule(_engine.GLProvider));

            _engine.GLProvider.LoadShader("Rectangle", "Rectangle");
            _engine.GLProvider.LoadShader("NineSlice", "NineSlice");

            _engine.Renderer.SetViewport(ClientSize.Width, ClientSize.Height);
            
            if (TryGetScene(out _scene)) _engine.SceneManager.SwitchTo(_scene);

            _stopwatch.Start();
            MainTimer.Start();

            Activate();
            Focus();
        }

        private void Restart()
        {

            string message = _scene.WinnerName == ""
                ? "ÍÈ×Üß! Îáà èãðîêà ïîãèáëè îäíîâðåìåííî!"
                : $"ÏÎÁÅÄÈË ÈÃÐÎÊ {_scene.WinnerName}!\n\nÏîçäðàâëÿåì ïîáåäèòåëÿ!";
            DialogResult result = MessageBox.Show(message + "\n\nÍà÷àòü íîâóþ èãðó?", "ÈÃÐÀ ÎÊÎÍ×ÅÍÀ",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _scene = new(_engine, "green", "red", "Levels/Arena1.json");
                _scene.OnEnd += Restart;
                _engine.SceneManager.ScheduleEndScene();
                _engine.SceneManager.ScheduleStartScene(_scene);

            }
            if (result == DialogResult.No) Environment.Exit(0);
        }

        private bool TryGetScene(out MainScene scene)
        {
            scene = default!;
            string[] variants = [.. PlayerFactory.Instance.GetIds()];
            using var form = new MenuForm(variants);
            if (form.ShowDialog() == DialogResult.OK)
            {
                scene = new(_engine, form.PlayerA, form.PlayerB, "Levels/Arena1.json");
                scene.OnEnd += Restart;
                return true;
            }
            return false;
        }

        private void OnGLLoad(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.ClearColor(Color.Black);

            MainGLControl.Enabled = false;
            _engine.Init();
        }

        private void OnGLPaint(object sender, PaintEventArgs e)
        {
            var _gl = (GLControl)sender;
            _gl.MakeCurrent();
            _engine.Render();
            _gl.SwapBuffers();
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
            _engine.Dispose();
        }

        private void MainForm_OnKeyDown(object sender, KeyEventArgs e) => _engine.Input.SetKeyDown(KeyConverter.ToOpenTK(e.KeyCode));
        private void MainForm_OnKeyUp(object sender, KeyEventArgs e) => _engine.Input.SetKeyUp(KeyConverter.ToOpenTK(e.KeyCode));
        private void OnLostGlobalFacus(object sender, EventArgs e) => _engine.Input.ClearStates();
    }
}
