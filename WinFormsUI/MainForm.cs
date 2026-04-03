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

            ClientSize = new Size(1280, 720);

            _engine = new GameEngine("D:\\!!GSTU\\C2\\S2\\!Kurs\\GunFight\\Assets");
        }

        private void OnGLLoad(object sender, EventArgs e)
        {
            GL.ClearColor(Color.White);
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
    }
}
