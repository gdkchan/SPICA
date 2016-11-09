using System.IO;
using System.Text;

namespace SPICA.Formats.GFL2.Utils
{
    static class GFString
    {
        public static string ReadLength(BinaryReader Reader, int Length)
        {
            StringBuilder SB = new StringBuilder();

            while (Length-- > 0)
            {
                char Chr = Reader.ReadChar();
                if (Chr != '\0') SB.Append(Chr);
            }

            return SB.ToString();
        }

        public static void WriteLength(BinaryWriter Writer, string Value, int Length)
        {
            int Index = 0;

            if (Value != null)
            {
                while (Index < Value.Length && Index++ < Length)
                {
                    Writer.Write(Value[Index]);
                }
            }

            while (Index++ < Length) Writer.Write((byte)0);
        }
    }
}
