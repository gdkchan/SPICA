using System.IO;
using System.Text;

namespace SPICA.Formats.GFL2.Utils
{
    static class GFString
    {
        public static string[] ReadArray(BinaryReader Reader, int Count)
        {
            string[] Output = new string[Count];

            for (int Index = 0; Index < Count; Index++)
            {
                Output[Index] = ReadLength(Reader, Reader.ReadByte());
            }

            return Output;
        }

        public static string ReadLength(BinaryReader Reader, int Length)
        {
            if (Length > 0)
            {
                StringBuilder SB = new StringBuilder();

                while (Length-- > 0)
                {
                    char Chr = Reader.ReadChar();
                    if (Chr != '\0') SB.Append(Chr);
                }

                return SB.ToString();
            }
            else
            {
                return null;
            }
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
