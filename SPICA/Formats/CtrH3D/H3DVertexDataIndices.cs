using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System.IO;

namespace SPICA.Formats.CtrH3D
{
    public struct H3DVertexDataIndices : ICustomSerialization
    {
        private byte Type;

        public PICADrawMode DrawMode;

        private ushort Count;

        public int MaxIndex
        {
            get
            {
                int Max = 0;

                foreach (ushort Index in Indices)
                {
                    if (Max < Index)
                        Max = Index;
                }

                return Max;
            }
        }

        [Ignore] public ushort[] Indices;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            bool Is16Bits = Type == 1;
            uint Address  = Deserializer.Reader.ReadUInt32();
            long Position = Deserializer.BaseStream.Position;

            Indices = new ushort[Count];

            Deserializer.BaseStream.Seek(Address, SeekOrigin.Begin);

            for (int Index = 0; Index < Count; Index++)
            {
                Indices[Index] = Is16Bits
                    ? Deserializer.Reader.ReadUInt16()
                    : Deserializer.Reader.ReadByte();
            }

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Writer.Write(Type);
            Serializer.Writer.Write((byte)DrawMode);
            Serializer.Writer.Write((ushort)Indices.Length);

            H3DSection Section = H3DSection.RawDataIndex16;

            object Data;

            if (MaxIndex <= byte.MaxValue)
            {
                Section = H3DSection.RawDataIndex8;

                byte[] Buffer = new byte[Indices.Length];

                for (int Index = 0; Index < Indices.Length; Index++)
                {
                    Buffer[Index] = (byte)Indices[Index];
                }

                Data = Buffer;
            }
            else
            {
                Data = Indices;
            }

            long Position = Serializer.BaseStream.Position;

            H3DRelocator.AddCmdReloc(Serializer, Section, Position);

            Serializer.Sections[(uint)H3DSectionId.RawData].Values.Add(new RefValue()
            {
                Parent   = this,
                Position = Position,
                Value    = Data
            });

            Serializer.BaseStream.Seek(4, SeekOrigin.Current);

            return true;
        }
    }
}
