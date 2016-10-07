using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;
using System.IO;

namespace SPICA.Formats.H3D.Model.Mesh
{
    class H3DSubMesh : ICustomDeserializer
    {
        public H3DSubMeshSkinning Skinning;
        public byte Padding;
        public ushort BoneIndicesCount;

        [FixedLength(20), Inline]
        public ushort[] BoneIndices;

        public uint[] Commands;

        public int MaxIndex
        {
            get
            {
                int Max = 0;

                foreach (ushort Index in Indices)
                {
                    if (Index > Max) Max = Index;
                }

                return Max;
            }
        }

        [NonSerialized]
        private byte[] RawBuffer;

        [NonSerialized]
        public ushort BoolUniforms;

        [NonSerialized]
        public ushort[] Indices;

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            uint BufferAddress = 0;
            uint BufferCount = 0;

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_VSH_BOOLUNIFORM: BoolUniforms = (ushort)Param; break;
                    case PICARegister.GPUREG_INDEXBUFFER_CONFIG: BufferAddress = Param; break;
                    case PICARegister.GPUREG_NUMVERTICES: BufferCount = Param; break;
                }
            }

            bool Format = (BufferAddress & (1u << 31)) != 0;
            long Position = Deserializer.BaseStream.Position;

            Indices = new ushort[BufferCount];

            Deserializer.BaseStream.Seek(BufferAddress & 0x7fffffff, SeekOrigin.Begin);

            for (int Index = 0; Index < BufferCount; Index++)
            {
                if (Format)
                {
                    Indices[Index] = Deserializer.Reader.ReadUInt16();
                }
                else
                {
                    Indices[Index] = Deserializer.Reader.ReadByte();
                }
            }

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }
    }
}
