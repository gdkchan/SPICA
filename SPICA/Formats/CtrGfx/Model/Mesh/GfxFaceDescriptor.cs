using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxFaceDescriptor : ICustomSerialization
    {
        private uint Flags;
        private uint Unk0;

        private byte[] RawBuffer;

        [Ignore] public ushort[] Indices;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            bool IsBuffer16Bits = (Flags & 2) != 0;

            Indices = new ushort[RawBuffer.Length >> (IsBuffer16Bits ? 1 : 0)];

            for (int i = 0; i < RawBuffer.Length; i += (IsBuffer16Bits ? 2 : 1))
            {
                if (IsBuffer16Bits)
                {
                    Indices[i >> 1] = (ushort)(
                        RawBuffer[i + 0] << 0 |
                        RawBuffer[i + 1] << 8);
                }
                else
                {
                    Indices[i] = RawBuffer[i];
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //TODO

            return false;
        }
    }
}
