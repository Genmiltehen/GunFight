using GameEngineLib.Impl;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Windows.Forms;

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
            _engine = new GameEngine();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            int width = ClientSize.Width;
            int height = ClientSize.Height;
            _engine.Renderer.SetViewport(width, height);


            _engine.SceneManager.SwitchTo(new TestScene(_engine.Assets));
            _engine.AddCamera();

            _stopwatch.Start();
            MainTimer.Start();
        }

        private void OnGLLoad(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.ClearColor(Color.Black);

            _engine.Init("D:\\!!GSTU\\C2\\S2\\!Kurs\\GunFight\\Assets");
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
            base.OnResize(e);

            int width = ClientSize.Width;
            int height = ClientSize.Height;

            if (width > 0 && height > 0) _engine.Renderer.SetViewport(width, height);
        }
    }
}
