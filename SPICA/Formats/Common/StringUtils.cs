using System.IO;
using System.Text;

namespace SPICA.Formats.Common
{
    static class StringUtils
    {
        //Read
        public static string ReadNullTerminatedString(this BinaryReader Reader)
        {
            StringBuilder SB = new StringBuilder();

            char Chr;
            while ((Chr = Reader.ReadChar()) != '\0')
            {
                SB.Append(Chr);
            }

            return SB.ToString();
        }

        public static string ReadPaddedString(this BinaryReader Reader, int Length)
        {
            if (Length > 0)
            {
                long Position = Reader.BaseStream.Position + Length;

                StringBuilder SB = new StringBuilder();

                while (Length-- > 0)
                {
                    char Chr = Reader.ReadChar();

                    if (Chr != '\0')
                        SB.Append(Chr);
                    else
                        break;
                }

                Reader.BaseStream.Seek(Position, SeekOrigin.Begin);

                return SB.ToString();
            }
            else
            {
                return null;
            }
        }

        public static string ReadIntLengthString(this BinaryReader Reader)
        {
            return ReadPaddedString(Reader, Reader.ReadInt32());
        }

        public static string ReadByteLengthString(this BinaryReader Reader)
        {
            return ReadPaddedString(Reader, Reader.ReadByte());
        }

        public static string[] ReadStringArray(this BinaryReader Reader, int Count)
        {
            string[] Output = new string[Count];

            for (int Index = 0; Index < Count; Index++)
            {
                Output[Index] = ReadByteLengthString(Reader);
            }

            return Output;
        }

        //Write
        public static void WritePaddedString(this BinaryWriter Writer, string Value, int Length)
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

        public static void WriteIntLengthString(this BinaryWriter Writer, string Value)
        {
            Writer.Write(Value.Length);
            WritePaddedString(Writer, Value, Value.Length);
        }

        public static void WriteByteLengthString(this BinaryWriter Writer, string Value)
        {
            Writer.Write((byte)Value.Length);
            WritePaddedString(Writer, Value, Value.Length);
        }

        public static void WriteStringArray(this BinaryWriter Writer, string[] Values)
        {
            foreach (string Value in Values)
            {
                WriteByteLengthString(Writer, Value);
            }
        }
    }
}
