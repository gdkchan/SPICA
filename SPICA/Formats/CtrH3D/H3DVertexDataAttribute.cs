using SPICA.Formats.Common;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

namespace SPICA.Formats.CtrH3D
{
    public struct H3DVertexDataAttribute : ICustomSerialization
    {
        public PICAAttributeName Name;

        private byte Type;

        public PICAAttributeFormat Format
        {
            get
            {
                return (PICAAttributeFormat)BitUtils.GetBits(Type, 0, 2);
            }
            set
            {
                Type = BitUtils.SetBits(Type, (uint)value, 0, 2);
            }
        }

        public int Elements
        {
            get
            {
                return (int)BitUtils.GetBits(Type, 2, 2);
            }
            set
            {
                Type = BitUtils.SetBits(Type, (uint)value, 2, 2);
            }
        }

        [Padding(2)] public byte Stride;

        public uint Offset;

        [Ignore] internal H3DVertexData Parent;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer) { }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.RawDataVtx.Values.Add(new RefValue
            {
                Position      = Serializer.BaseStream.Position + 4,
                Value         = Parent.RawBuffer,
                PointerOffset = Offset
            });

            return false;
        }
    }
}
