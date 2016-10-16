using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;
using SPICA.Utils;

using System;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.H3D.Model.Mesh
{
    [Repeatable]
    struct H3DMesh : ICustomSerialization, ICustomSerializeCmd
    {
        public ushort MaterialIndex;
        public byte Flags;
        public byte Padding;
        public ushort NodeIndex;
        public ushort Key;

        public H3DMeshType Type
        {
            get { return (H3DMeshType)BitUtils.GetBits(Flags, 0, 2); }
            set { Flags = BitUtils.SetBits(Flags, (uint)value, 0, 2); }
        }

        public H3DMeshSkinning Skinning
        {
            get { return (H3DMeshSkinning)BitUtils.GetBits(Flags, 2, 2); }
            set { Flags = BitUtils.SetBits(Flags, (uint)value, 2, 2); }
        }

        public uint Priority
        {
            get { return BitUtils.GetBits(Key, 0, 8); }
            set { Key = BitUtils.SetBits(Key, value, 0, 8); }
        }

        public uint Layer
        {
            get { return BitUtils.GetBits(Key, 8, 2); }
            set { Key = BitUtils.SetBits(Key, value, 8, 2); }
        }

        public uint[] EnableCommands;

        public List<H3DSubMesh> SubMeshes;

        public uint[] DisableCommands;

        public Vector3D MeshCenter;

        public H3DModel Parent;

        public uint UserDefinedAddress;

        public H3DMetaData MetaData;

        [NonSerialized]
        public byte[] RawBuffer;

        [NonSerialized]
        public int VertexStride;

        [NonSerialized]
        public PICAAttribute[] Attributes;

        [NonSerialized]
        public Vector4D PositionOffset;

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(EnableCommands);

            uint BufferAddress = 0;
            uint BufferFormats = 0;
            byte BufferStride = 0;
            byte AttributesCount = 0;
            ulong BufferAttributes = 0;
            ulong BufferPermutation = 0;

            int UniformIndex = 0;

            Vector4D[] Uniform = new Vector4D[96];

            //Default Offset and Scale
            Uniform[6] = new Vector4D(0, 0, 0, 0);
            Uniform[7] = new Vector4D(1, 1, 1, 1);
            Uniform[8] = new Vector4D(1, 1, 1, 1);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_LOW: BufferFormats |= Param; break;
                    case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_HIGH: BufferFormats |= Param << 32; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_OFFSET: BufferAddress = Param; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG1: BufferAttributes |= Param; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG2:
                        BufferAttributes |= (Param & 0xffff) << 32;
                        BufferStride = (byte)(Param >> 16);
                        AttributesCount = (byte)(Param >> 28);
                        break;
                    case PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_LOW: BufferPermutation |= Param; break;
                    case PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_HIGH: BufferPermutation |= Param << 32; break;
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX: UniformIndex = (int)((Param & 0xff) << 2); break;
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA0:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA1:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA2:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA3:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA4:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA5:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA6:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA7:
                        int ParamIndex = 0;

                        while (ParamIndex < Cmd.Parameters.Length)
                        {
                            switch (UniformIndex & 3)
                            {
                                case 0: Uniform[UniformIndex++ >> 2].W = IOUtils.ToFloat(Cmd.Parameters[ParamIndex++]); break;
                                case 1: Uniform[UniformIndex++ >> 2].Z = IOUtils.ToFloat(Cmd.Parameters[ParamIndex++]); break;
                                case 2: Uniform[UniformIndex++ >> 2].Y = IOUtils.ToFloat(Cmd.Parameters[ParamIndex++]); break;
                                case 3: Uniform[UniformIndex++ >> 2].X = IOUtils.ToFloat(Cmd.Parameters[ParamIndex++]); break;
                            }
                        }
                        break;
                }
            }

            Attributes = new PICAAttribute[AttributesCount];

            for (int Index = 0; Index < AttributesCount; Index++)
            {
                int PermutationIdx = (int)((BufferAttributes >> Index * 4) & 0xf);
                int AttributeFmt = (int)((BufferFormats >> PermutationIdx * 4) & 0xf);

                PICAAttribute Attrib = new PICAAttribute();

                Attrib.Name = (PICAAttributeName)((BufferPermutation >> PermutationIdx * 4) & 0xf);
                Attrib.Format = (PICAAttributeFormat)(AttributeFmt & 3);
                Attrib.Elements = (AttributeFmt >> 2) + 1;
                Attrib.Scale = 1;

                switch (Attrib.Name)
                {
                    case PICAAttributeName.Position: Attrib.Scale = Uniform[7].X; break;
                    case PICAAttributeName.Normal: Attrib.Scale = Uniform[7].Y; break;
                    case PICAAttributeName.Tangent: Attrib.Scale = Uniform[7].Z; break;
                    case PICAAttributeName.Color: Attrib.Scale = Uniform[7].W; break;
                    case PICAAttributeName.TextureCoordinate0: Attrib.Scale = Uniform[8].X; break;
                    case PICAAttributeName.TextureCoordinate1: Attrib.Scale = Uniform[8].Y; break;
                    case PICAAttributeName.TextureCoordinate2: Attrib.Scale = Uniform[8].Z; break;
                    case PICAAttributeName.BoneWeight: Attrib.Scale = Uniform[8].W; break;
                }

                Attributes[Index] = Attrib;
            }

            long Position = Deserializer.BaseStream.Position;
            int BufferCount = 0;

            //The PICA doesn't need the total number of Attributes on the Buffer, so it is not present on the Commands
            //So we need to get the Max Index used on the Index Buffer to figure out the total number of Attributes
            foreach (H3DSubMesh SM in SubMeshes)
            {
                if (SM.MaxIndex > BufferCount) BufferCount = SM.MaxIndex;
            }

            BufferCount++;
            PositionOffset = Uniform[6];
            VertexStride = BufferStride;

            Deserializer.BaseStream.Seek(BufferAddress, SeekOrigin.Begin);

            RawBuffer = Deserializer.Reader.ReadBytes(BufferCount * BufferStride);

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public PICAVertex[] GetVertices()
        {
            return VerticesConverter.GetVertices(RawBuffer, VertexStride, Attributes);
        }

        public bool Serialize(BinarySerializer Serializer)
        {
            //TODO

            return false;
        }

        public void SerializeCmd(BinarySerializer Serializer, object Value)
        {
            if (Value == EnableCommands)
            {
                long Position = Serializer.BaseStream.Position;

                Serializer.RawDataVtx.Values.Add(new RefValue
                {
                    Value = RawBuffer,
                    Position = Position + 0x30
                });

                Serializer.Pointers.Add(Position + 0x20);

                Serializer.Relocator.RelocTypes.Add(Position + 0x20, H3DRelocationType.BaseAddress);
                Serializer.Relocator.RelocTypes.Add(Position + 0x30, H3DRelocationType.RawDataVertex);
            }
        }
    }
}
