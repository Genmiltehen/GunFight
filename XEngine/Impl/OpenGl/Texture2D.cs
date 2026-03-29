using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace GameEngineLib.Impl.OpenGl
{
    public class Texture2D
    {
        public int Handle { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        private bool _disposed;

        public bool IsValid => !_disposed;

        public static Texture2D FromPath(string path)
        {
            using var image = new Bitmap(path);
            return FromBitmap(image);
        }

        private static Texture2D FromBitmap(Bitmap bitmap)
        {
            int texHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texHandle);

            using (var cloned = new Bitmap(bitmap))
            {
                // INFO: Potential flip culprit
                cloned.RotateFlip(RotateFlipType.RotateNoneFlipY);

                var data = cloned.LockBits(
                    new Rectangle(0, 0, cloned.Width, cloned.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
                GL.PixelStore(PixelStoreParameter.UnpackRowLength, data.Stride / 4);

                GL.TexImage2D(
                    TextureTarget.Texture2D, 0,
                    PixelInternalFormat.Rgba,
                    data.Width, data.Height, 0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0
                );

                cloned.UnlockBits(data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return new Texture2D { Handle = texHandle, Width = bitmap.Width, Height = bitmap.Height };
        }

        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Dispose()
        {
            if (_disposed) return;
            GL.DeleteTexture(Handle);
            _disposed = true;
        }
    }
}
