using System;
using System.IO;
using System.Text;

namespace SPICA.Formats.Common
{
    public static class StringUtils
    {
        //Read
        public static byte[] ReadNullTerminatedByteArray(this BinaryReader Reader)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                for (byte Value; (Value = Reader.ReadByte()) != 0;)
                {
                    MS.WriteByte(Value);
                }

                return MS.ToArray();
            }
        }

        public static byte[] ReadNullTerminatedShortLEByteArray(this BinaryReader Reader)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                for (ushort Value; (Value = Reader.ReadUInt16()) != 0;)
                {
                    MS.WriteByte((byte)(Value >> 0));
                    MS.WriteByte((byte)(Value >> 8));
                }

                return MS.ToArray();
            }
        }

        public static byte[] ReadNullTerminatedShortBEByteArray(this BinaryReader Reader)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                for (ushort Value; (Value = Reader.ReadUInt16()) != 0;)
                {
                    MS.WriteByte((byte)(Value >> 8));
                    MS.WriteByte((byte)(Value >> 0));
                }

                return MS.ToArray();
            }
        }

        public static string ReadNullTerminatedString(this BinaryReader Reader)
        {
            return Encoding.ASCII.GetString(Reader.ReadNullTerminatedByteArray());
        }

        public static string ReadNullTerminatedString(this BinaryReader Reader, int CodePage)
        {
            return Encoding.GetEncoding(CodePage).GetString(Reader.ReadNullTerminatedByteArray());
        }

        public static string ReadNullTerminatedStringSJis(this BinaryReader Reader)
        {
            return Reader.ReadNullTerminatedString(932);
        }

        public static string ReadNullTerminatedStringUtf8(this BinaryReader Reader)
        {
            return Encoding.UTF8.GetString(Reader.ReadNullTerminatedByteArray());
        }

        public static string ReadNullTerminatedStringUtf16LE(this BinaryReader Reader)
        {
            return Encoding.Unicode.GetString(Reader.ReadNullTerminatedShortLEByteArray());
        }

        public static string ReadNullTerminatedStringUtf16BE(this BinaryReader Reader)
        {
            return Encoding.Unicode.GetString(Reader.ReadNullTerminatedShortBEByteArray());
        }

        public static string ReadPaddedString(this BinaryReader Reader, int Length)
        {
            if (Length > 0)
            {
                long Position = Reader.BaseStream.Position + Length;

                StringBuilder SB = new StringBuilder();

                for (char Value; Length-- > 0 && (Value = Reader.ReadChar()) != '\0';)
                {
                    SB.Append(Value);
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
        public static void WriteNullTerminatedStringUtf8(this BinaryWriter Writer, string Value)
        {
            Writer.Write(Encoding.UTF8.GetBytes(Value ?? "" + '\0'));
        }

        public static void WriteNullTerminatedStringUtf16LE(this BinaryWriter Writer, string Value)
        {
            Writer.Write(Encoding.Unicode.GetBytes(Value ?? "" + '\0'));
        }

        public static void WriteNullTerminatedStringUtf16BE(this BinaryWriter Writer, string Value)
        {
            Writer.Write(IOUtils.ByteSwap16(Encoding.Unicode.GetBytes(Value ?? "" + '\0')));
        }

        public static void WritePaddedString(this BinaryWriter Writer, string Value, int Length)
        {
            int Index = 0;

            if (Value != null && Value.Length > 0)
            {
                while (Index < Math.Min(Value.Length, Length))
                {
                    Writer.Write(Value[Index++]);
                }
            }

            while (Index++ < Length) Writer.Write((byte)0);
        }

        public static void WriteIntLengthString(this BinaryWriter Writer, string Value)
        {
            int Length = Value?.Length ?? 0;

            Writer.Write(Length);

            WritePaddedString(Writer, Value, Length);
        }

        public static void WriteByteLengthString(this BinaryWriter Writer, string Value)
        {
            int Length = Value?.Length ?? 0;

            Writer.Write((byte)Length);

            WritePaddedString(Writer, Value, Length);
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
