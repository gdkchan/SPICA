using SPICA.Formats.Common;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System;
using System.IO;

namespace SPICA.Formats.CtrH3D.Model.Mesh
{
    [Inline]
    public class H3DSubMesh : ICustomSerialization, ICustomSerializeCmd
    {
        [Padding(2)] public H3DSubMeshSkinning Skinning;

        public ushort BoneIndicesCount;

        [FixedLength(20), Inline] private ushort[] _BoneIndices;

        public ushort[] BoneIndices
        {
            get => _BoneIndices;
            set
            {
                if (value == null)
                {
                    throw Exceptions.GetNullException("BoneIndices");
                }

                if (value.Length > 20)
                {
                    throw Exceptions.GetGreaterThanException("BoneIndices", 20);
                }

                if (value.Length < 20)
                {
                    Array.Copy(value, _BoneIndices, value.Length);
                }
                else
                {
                    _BoneIndices = value;
                }

                BoneIndicesCount = (ushort)value.Length;
            }
        }

        private uint[] Commands;

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

        [Ignore] public ushort BoolUniforms;

        [Ignore] public ushort[] Indices;

        [Ignore] public PICAPrimitiveMode PrimitiveMode;

        public H3DSubMesh()
        {
            _BoneIndices = new ushort[20];
        }

        public H3DSubMesh(ushort[] Indices) : this()
        {
            this.Indices = Indices;
        }

        public H3DSubMesh(ushort[] Indices, ushort[] BoneIndices, H3DSubMeshSkinning Skinning) : this()
        {
            this.Indices     = Indices;
            this.BoneIndices = BoneIndices;
            this.Skinning    = Skinning;
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            uint BufferAddress = 0;
            uint BufferCount   = 0;

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_VSH_BOOLUNIFORM:    BoolUniforms  = (ushort)Param; break;
                    case PICARegister.GPUREG_INDEXBUFFER_CONFIG: BufferAddress =         Param; break;
                    case PICARegister.GPUREG_NUMVERTICES:        BufferCount   =         Param; break;
                    case PICARegister.GPUREG_PRIMITIVE_CONFIG:
                        PrimitiveMode = (PICAPrimitiveMode)(Param >> 8);
                        break;
                }
            }

            bool Is16BitsIdx = (BufferAddress >> 31) != 0;
            long Position    = Deserializer.BaseStream.Position;

            Indices = new ushort[BufferCount];

            Deserializer.BaseStream.Seek(BufferAddress & 0x7fffffff, SeekOrigin.Begin);

            for (int Index = 0; Index < BufferCount; Index++)
            {
                if (Is16BitsIdx)
                    Indices[Index] = Deserializer.Reader.ReadUInt16();
                else
                    Indices[Index] = Deserializer.Reader.ReadByte();
            }

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
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

            Writer.SetCommand(PICARegister.GPUREG_PRIMITIVE_CONFIG, (uint)PrimitiveMode << 8, 8);
            Writer.SetCommand(PICARegister.GPUREG_PRIMITIVE_CONFIG, (uint)PrimitiveMode << 8, 8);

            Writer.WriteEnd();

            Commands = Writer.GetBuffer();

            return false;
        }

        void ICustomSerializeCmd.SerializeCmd(BinarySerializer Serializer, object Value)
        {
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

            long Position = Serializer.BaseStream.Position + 0x10;

            H3DRelocator.AddCmdReloc(Serializer, Section, Position);

            Serializer.Sections[(uint)H3DSectionId.RawData].Values.Add(new RefValue()
            {
                Parent   = this,
                Value    = Data,
                Position = Position
            });
        }
    }
}
