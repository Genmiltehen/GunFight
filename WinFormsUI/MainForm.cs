using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;

namespace WinFormsUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            MainGLControl.Load += OnGLLoad;

        }

        private void OnGLLoad(object sender, EventArgs e)
        {
            GL.ClearColor(Color.CornflowerBlue);
        }

        private void OnGLPaint(object sender, PaintEventArgs e)
        {
            MainGLControl.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // --- YOUR RENDERING CODE GOES HERE ---

            MainGLControl.SwapBuffers();
        }
    }
}
