using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D.Texture;

using System;

namespace SPICA.Renderer
{
    public class Texture : IDisposable
    {
        public string Name;

        public int Id;

        public Texture(RenderEngine Renderer, H3DTexture Texture)
        {
            Name = Texture.Name;

            Id = GL.GenTexture();

            if (Texture.IsCubeTexture)
            {
                GL.BindTexture(TextureTarget.TextureCubeMap, Id);

                for (int Face = 0; Face < 6; Face++)
                {
                    GL.TexImage2D(TextureTarget2d.TextureCubeMapPositiveX + Face,
                        0,
                        TextureComponentCount.Rgba,
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

                GL.TexImage2D(TextureTarget2d.Texture2D,
                    0,
                    TextureComponentCount.Rgba,
                    (int)Texture.Width,
                    (int)Texture.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    Texture.ToRGBA());
            }
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
