using SPICA.Serialization;

using System;

namespace SPICA.Formats.H3D
{
    struct H3DVertexData : ICustomSerialization
    {
        public byte AttributesCount;
        private byte Padding;
        public ushort IndicesCount;

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            uint AttributesAddress = Deserializer.Reader.ReadUInt32();
            uint IndicesAddress = Deserializer.Reader.ReadUInt32();

            //TODO
        }

        public bool Serialize(BinarySerializer Serializer)
        {
            throw new NotImplementedException();
        }
    }
}
