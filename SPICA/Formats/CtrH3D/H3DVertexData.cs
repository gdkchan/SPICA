using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;

namespace SPICA.Formats.CtrH3D
{
    public struct H3DVertexData : ICustomSerialization
    {
        [Padding(2)] public byte AttributesCount;

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
