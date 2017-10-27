using SPICA.Formats.CtrH3D;

using System.IO;
using System.Text;
using System;
using SPICA.Formats.GFL2.Texture;
using SPICA.Formats.CtrH3D.Texture;

namespace SPICA.WinForms.Formats
{
    class GFPackedTexture
    {
		private GFTexture Texture;

		public GFPackedTexture(H3D Scene, int index) {
			Texture = new GFTexture(Scene.Textures[index]);
		}

        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header, int StartIndex)
        {
            H3D Output = new H3D();

            //Textures and animations
            for (int i = StartIndex; i < Header.Entries.Length; i++)
            {
                byte[] Buffer = new byte[Header.Entries[i].Length];

                Input.Seek(Header.Entries[i].Address, SeekOrigin.Begin);

                Input.Read(Buffer, 0, Buffer.Length);

                if (Buffer.Length < 4 || Encoding.ASCII.GetString(Buffer, 0, 4) != "BCH\0") continue;

                using (MemoryStream MS = new MemoryStream(Buffer))
                {
                    Output.Merge(H3D.Open(MS));
                }
            }

            return Output;
        }

		public void Save(string FileName) {
			using (BinaryWriter br = new BinaryWriter(new FileStream(FileName, FileMode.Create))) {
				Texture.Write(br);
			}
		}
	}
}
