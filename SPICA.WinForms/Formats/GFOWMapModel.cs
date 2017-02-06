using SPICA.Formats.CtrH3D;

using System.IO;

namespace SPICA.WinForms.Formats
{
    class GFOWMapModel
    {
        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header)
        {
            H3D Output;

            //Model
            byte[] Buffer = new byte[Header.Entries[1].Length];

            Input.Seek(Header.Entries[1].Address, SeekOrigin.Begin);

            Input.Read(Buffer, 0, Buffer.Length);

            using (MemoryStream MS = new MemoryStream(Buffer))
            {
                Output = H3D.Open(MS);
            }

            return Output;
        }
    }
}
