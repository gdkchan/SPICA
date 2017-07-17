using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System.IO;

namespace SPICA.Formats.CtrH3D
{
    public struct H3DVertexData : ICustomSerialization
    {
        [Ignore] public H3DVertexDataAttribute[] Attributes;
        [Ignore] public H3DVertexDataIndices[]   Indices;

        public int VertexStride { get; private set; }

        [Ignore] public byte[] RawBuffer;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Attributes = new H3DVertexDataAttribute[(byte)Deserializer.Reader.ReadUInt16()];
            Indices    = new H3DVertexDataIndices[Deserializer.Reader.ReadUInt16()];

            uint AttributesAddress = Deserializer.Reader.ReadUInt32();
            uint IndicesAddress    = Deserializer.Reader.ReadUInt32();

            Deserializer.BaseStream.Seek(AttributesAddress, SeekOrigin.Begin);

            int BaseAddress = int.MaxValue;

            for (int Index = 0; Index < Attributes.Length; Index++)
            {
                Attributes[Index] = Deserializer.Deserialize<H3DVertexDataAttribute>();
                
                if (!Attributes[Index].IsFixed &&
                     Attributes[Index].Offset < BaseAddress)
                {
                    BaseAddress = Attributes[Index].Offset;
                }
            }

            VertexStride = 0;

            for (int Index = 0; Index < Attributes.Length; Index++)
            {
                if (!Attributes[Index].IsFixed)
                {
                    Attributes[Index].Offset -= BaseAddress;

                    int Size = Attributes[Index].Elements;

                    switch (Attributes[Index].Format)
                    {
                        case PICAAttributeFormat.Short: Size <<= 1; break;
                        case PICAAttributeFormat.Float: Size <<= 2; break;
                    }

                    VertexStride += Size;
                }
            }

            Deserializer.BaseStream.Seek(IndicesAddress, SeekOrigin.Begin);

            for (int Index = 0; Index < Indices.Length; Index++)
            {
                Indices[Index] = Deserializer.Deserialize<H3DVertexDataIndices>();
            }

            int BufferCount = 0;

            //The PICA doesn't need the total number of Attributes on the Buffer, so it is not present on the Commands
            //So we need to get the Max Index used on the Index Buffer to figure out the total number of Attributes
            foreach (H3DVertexDataIndices SM in Indices)
            {
                if (BufferCount < SM.MaxIndex)
                    BufferCount = SM.MaxIndex;
            }

            BufferCount++;

            Deserializer.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);

            RawBuffer = Deserializer.Reader.ReadBytes(BufferCount * VertexStride);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            for (int Index = 0; Index < Attributes.Length; Index++)
            {
                Attributes[Index].RawBuffer = RawBuffer;
            }

            Serializer.Writer.Write((ushort)Attributes.Length);
            Serializer.Writer.Write((ushort)Indices.Length);

            Serializer.Sections[(uint)H3DSectionId.Contents].Values.Add(new RefValue()
            {
                Position = Serializer.BaseStream.Position,
                Value    = Attributes
            });

            Serializer.Sections[(uint)H3DSectionId.Contents].Values.Add(new RefValue()
            {
                Position = Serializer.BaseStream.Position + 4,
                Value    = Indices
            });

            Serializer.BaseStream.Seek(8, SeekOrigin.Current);

            return true;
        }
    }
}
