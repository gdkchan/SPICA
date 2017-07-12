using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D.Texture;

using System;

namespace SPICA.Rendering
{
    public class Texture : IDisposable
    {
        public string Name;

        public int Id;

        private TextureTarget Target;

        public Texture(H3DTexture Texture)
        {
            Name = Texture.Name;

            Id = GL.GenTexture();

            Target = Texture.IsCubeTexture
                ? TextureTarget.TextureCubeMap
                : TextureTarget.Texture2D;

            if (Texture.IsCubeTexture)
            {
                GL.BindTexture(TextureTarget.TextureCubeMap, Id);

                for (int Face = 0; Face < 6; Face++)
                {
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + Face,
                        0,
                        PixelInternalFormat.Rgba,
                        (int)Texture.Width,
                        (int)Texture.Height,
                        0,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        Texture.ToRGBA(Face));
                }
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, Id);

                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    (int)Texture.Width,
                    (int)Texture.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    Texture.ToRGBA());
            }
        }

        public void Bind(int Unit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + Unit);

            GL.BindTexture(Target, Id);
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteTexture(Id);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
