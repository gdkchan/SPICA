using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.BinaryAttributes;

using System;
using System.IO;

namespace SPICA.Formats.H3D.Contents.Model
{
    class H3DFace : ICustomDeserializer, ICustomSerializer
    {
        public H3DSkinningMode Skinning;
        private byte Padding;

        [CountOf("BoneIndices")]
        public ushort BoneIndicesCount;

        [FixedCount(20)]
        public ushort[] BoneIndices;

        [PointerOf("IndicesBufferCommands")]
        private uint IndicesBufferCommandsAddress;

        [CountOf("IndicesBufferCommands")]
        private uint IndicesBufferCommandsCount;

        [TargetSection("CommandsSection")]
        private uint[] IndicesBufferCommands;

        [TargetSection("RawDataSection")]
        private byte[] RawBuffer;

        [NonSerialized]
        public ushort[] Indices;

        [NonSerialized]
        public ushort MaxIndex;

        public ushort this[int Index]
        {
            get { return Indices[Index]; }
            set { Indices[Index] = value; }
        }

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(IndicesBufferCommands);

            uint BufferAddress = 0;
            uint BufferCount = 0;

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_INDEXBUFFER_CONFIG: BufferAddress = Param; break;
                    case PICARegister.GPUREG_NUMVERTICES: BufferCount = Param; break;
                }
            }

            bool Format = (BufferAddress & (1u << 31)) != 0;
            long Position = Deserializer.BaseStream.Position;

            Indices = new ushort[BufferCount];
            MaxIndex = 0;

            Deserializer.BaseStream.Seek(BufferAddress & 0x7fffffff, SeekOrigin.Begin);

            for (int Index = 0; Index < BufferCount; Index++)
            {
                if (Format)
                    Indices[Index] = Deserializer.Reader.ReadUInt16();
                else
                    Indices[Index] = Deserializer.Reader.ReadByte();

                if (Indices[Index] > MaxIndex) MaxIndex = Indices[Index];
            }

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public void Serialize(BinarySerializer Serializer)
        {
            bool Format = false;

            foreach (ushort Index in Indices)
            {
                if (Index > byte.MaxValue)
                {
                    Format = true;
                    break;
                }
            }

            RawBuffer = new byte[Indices.Length * (Format ? 2 : 1)];

            for (int Index = 0; Index < Indices.Length; Index++)
            {
                if (Format)
                {
                    RawBuffer[(Index << 1)] = (byte)Indices[Index];
                    RawBuffer[(Index << 1) | 1] = (byte)(Indices[Index] >> 8);
                }
                else
                {
                    RawBuffer[Index] = (byte)Indices[Index];
                }
            }
        }
    }
}
