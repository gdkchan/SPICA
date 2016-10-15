using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System;
using System.IO;

namespace SPICA.Formats.H3D.Model.Mesh
{
    struct H3DSubMesh : ICustomSerialization, ICustomSerializeCmd
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
                    Indices[Index] = Deserializer.Reader.ReadUInt16();
                else
                    Indices[Index] = Deserializer.Reader.ReadByte();
            }

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public bool Serialize(BinarySerializer Serializer)
        {
            PICACommandWriter Writer = new PICACommandWriter();

            Writer.SetCommand(PICARegister.GPUREG_VSH_BOOLUNIFORM, BoolUniforms | 0x7fff0000u);
            Writer.SetCommand(PICARegister.GPUREG_RESTART_PRIMITIVE, true);
            Writer.SetCommand(PICARegister.GPUREG_INDEXBUFFER_CONFIG, 0);
            Writer.SetCommand(PICARegister.GPUREG_NUMVERTICES, (uint)Indices.Length);
            Writer.SetCommand(PICARegister.GPUREG_START_DRAW_FUNC0, false, 1);
            Writer.SetCommand(PICARegister.GPUREG_DRAWELEMENTS, true);
            Writer.SetCommand(PICARegister.GPUREG_START_DRAW_FUNC0, true, 1);
            Writer.SetCommand(PICARegister.GPUREG_VTX_FUNC, true);
            Writer.SetCommand(PICARegister.GPUREG_PRIMITIVE_CONFIG, 0, 8);
            Writer.SetCommand(PICARegister.GPUREG_PRIMITIVE_CONFIG, 0, 8);
            Writer.SetCommand(PICARegister.GPUREG_DUMMY, 0, 0);
            Writer.SetCommand(PICARegister.GPUREG_CMDBUF_JUMP1, true);

            Commands = Writer.GetBuffer();

            return false;
        }

        public void SerializeCmd(BinarySerializer Serializer, object Value)
        {
            H3DRelocationType RelocType = H3DRelocationType.RawDataIndex16;

            object Data;

            if (MaxIndex <= byte.MaxValue)
            {
                RelocType = H3DRelocationType.RawDataIndex8;

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

            long Position = Serializer.BaseStream.Position + 0x10;

            Serializer.RawDataVtx.Values.Add(new RefValue
            {
                Value = Data,
                Position = Position
            });

            Serializer.Relocator.RelocTypes.Add(Position, RelocType);
        }
    }
}
