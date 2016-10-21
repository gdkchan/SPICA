using SPICA.Serialization;

using System;

namespace SPICA.Formats.H3D
{
    public struct H3DVertexData : ICustomSerialization
    {
        public byte AttributesCount;
        private byte Padding;
        public ushort IndicesCount;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            uint AttributesAddress = Deserializer.Reader.ReadUInt32();
            uint IndicesAddress = Deserializer.Reader.ReadUInt32();

            //TODO
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            throw new NotImplementedException();
        }
    }
}
