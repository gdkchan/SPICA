using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Texture;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;

namespace SPICA.WinForms
{
    static class TextureManager
    {
        private static
            Dictionary<int, Bitmap> TextureCache = new
            Dictionary<int, Bitmap>();

        private static H3DDict<H3DTexture> _Textures;

        public static H3DDict<H3DTexture> Textures
        {
            get
            {
                return _Textures;
            }
            set
            {
                _Textures = value;

                _Textures.CollectionChanged += (sender, e) => { FlushCache(); };

                FlushCache();
            }
        }

        public static Bitmap GetTexture(int Index)
        {
            Bitmap Output = null;

            if (TextureCache.ContainsKey(Index))
            {
                Output = TextureCache[Index];
            }
            else if (Index > -1 && (_Textures?.Count ?? 0) > Index)
            {
                Output = _Textures[Index].ToBitmap();

                TextureCache.Add(Index, Output);
            }

            return Output;
        }

        public static void FlushCache()
        {
            foreach (Bitmap Bmp in TextureCache.Values)
            {
                Bmp.Dispose();
            }

            TextureCache.Clear();
        }
    }
}
