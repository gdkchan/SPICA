using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.IO;
using System.Text;

namespace SPICA.Formats.CtrH3D
{
    public class H3DStringUtf16 : ICustomSerialization
    {
        [Ignore] private string Str;

        public H3DStringUtf16() { }

        public H3DStringUtf16(string Str)
        {
            this.Str = Str;
        }

        public override string ToString()
        {
            return Str ?? string.Empty;
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                for (ushort Chr; (Chr = Deserializer.Reader.ReadUInt16()) != 0;)
                {
                    MS.WriteByte((byte)(Chr >> 0));
                    MS.WriteByte((byte)(Chr >> 8));
                }

                Str = Encoding.Unicode.GetString(MS.ToArray());
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Writer.Write(Encoding.Unicode.GetBytes(Str + '\0'));

            return true;
        }
    }
}
