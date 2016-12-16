using System.IO;

namespace SPICA.Formats.GFL
{
    class GFUtils
    {
        public static void Align(BinaryReader Reader)
        {
            while ((Reader.BaseStream.Position & 3) != 0) Reader.ReadByte();
        }
    }
}
