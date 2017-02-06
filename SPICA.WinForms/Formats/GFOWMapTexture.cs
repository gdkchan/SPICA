using SPICA.Formats.CtrH3D;

using System.IO;
using System.Text;

namespace SPICA.WinForms.Formats
{
    class GFOWMapTexture
    {
        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header)
        {
            H3D Output = new H3D();

            //Textures and animations
            for (int i = 1; i < Header.Entries.Length; i++)
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
    }
}
