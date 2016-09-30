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

        [TargetSection("CommandsSection"), CustomSerialization]
        private uint[] IndicesBufferCommands;

        [TargetSection("RawDataSection", 1)]
        private byte[] RawBuffer;

        [NonSerialized]
        public ushort BoolUniforms;

        [NonSerialized]
        public ushort[] Indices;

        [NonSerialized]
        public ushort MaxIndex;

        public ushort this[int Index]
        {
            get { return Indices[Index]; }
            set { Indices[Index] = value; }
        }

        public void Deserialize(BinaryDeserializer Deserializer, string FName)
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
                    case PICARegister.GPUREG_VSH_BOOLUNIFORM: BoolUniforms = (ushort)Param; break;
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

        public object Serialize(BinarySerializer Serializer, string FName)
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

            H3DRelocationType RType = Format ? H3DRelocationType.RawDataIndex16 : H3DRelocationType.RawDataIndex8;

            Serializer.AddPointer(RawBuffer, Serializer.BaseStream.Position + 0x10, typeof(uint));
            Serializer.Relocator.AddPointer(Serializer.BaseStream.Position + 0x10, (int)RType);

            return Writer.GetBuffer();
        }
    }
}
