using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx
{
    public class GfxStringUtf16BE : ICustomSerialization
    {
        [Ignore] private string Str;

        public GfxStringUtf16BE() { }

        public GfxStringUtf16BE(string Str)
        {
            this.Str = Str;
        }

        public override string ToString()
        {
            return Str ?? string.Empty;
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Str = Deserializer.Reader.ReadNullTerminatedStringUtf16BE();
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Writer.WriteNullTerminatedStringUtf16BE(Str);

            return true;
        }
    }
}
