using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D.LUT;

using System;
using System.Collections.Generic;

namespace SPICA.Rendering
{
    public class LUT : IDisposable
    {
        public string Name { get; private set; }

        private Dictionary<string, int> Ids;

        public LUT(H3DLUT LUT)
        {
            Name = LUT.Name;

            Ids = new Dictionary<string, int>();

            foreach (H3DLUTSampler Sampler in LUT.Samplers)
            {
                float[] Table = new float[512];

                if ((Sampler.Flags & H3DLUTFlags.IsAbsolute) != 0)
                {
                    for (int i = 0; i < 256; i++)
                    {
                        Table[i + 256] = Sampler.Table[i];
                        Table[i +   0] = Sampler.Table[0];
                    }
                }
                else
                {
                    for (int i = 0; i < 256; i += 2)
                    {
                        int PosIdx = i >> 1;
                        int NegIdx = PosIdx + 128;

                        Table[i + 256] = Sampler.Table[PosIdx];
                        Table[i + 257] = Sampler.Table[PosIdx];
                        Table[i +   0] = Sampler.Table[NegIdx];
                        Table[i +   1] = Sampler.Table[NegIdx];
                    }
                }

                int Id = GL.GenTexture();

                Ids.Add(Sampler.Name, Id);

                //Note: Use 2D instead of 1D textures for LUTs because GLES doesn't support 1D textures.
                GL.BindTexture(TextureTarget.Texture2D, Id);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.R32f,
                    512,
                    1,
                    0,
                    PixelFormat.Red,
                    PixelType.Float,
                    Table);
            }
        }

        public bool BindSampler(int Unit, string SamplerName)
        {
            if (Ids.ContainsKey(SamplerName))
            {
                GL.ActiveTexture(TextureUnit.Texture0 + Unit);
                GL.BindTexture(TextureTarget.Texture2D, Ids[SamplerName]);

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                foreach (int Id in Ids.Values)
                {
                    GL.DeleteTexture(Id);
                }

                Ids.Clear();

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
