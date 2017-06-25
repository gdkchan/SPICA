using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

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
            Str = Deserializer.Reader.ReadNullTerminatedStringUtf16LE();
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Writer.WriteNullTerminatedStringUtf16LE(Str);

            return true;
        }
    }
}
