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
using System.Linq;

namespace SPICA.Formats.H3D.Model.Mesh
{
    [Inline]
    public class H3DMesh : ICustomSerialization, ICustomSerializeCmd
    {
        public ushort MaterialIndex;
        private byte Flags;
        private byte Padding;
        public ushort NodeIndex;
        private ushort Key;

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

        private uint[] EnableCommands;

        public List<H3DSubMesh> SubMeshes;

        private uint[] DisableCommands;

        public Vector3D MeshCenter;

        public H3DModel Parent;

        private uint UserDefinedAddress;

        public H3DMetaData MetaData;

        [NonSerialized]
        public byte[] RawBuffer;

        [NonSerialized]
        public int VertexStride;

        [NonSerialized]
        public PICAAttribute[] Attributes;

        [NonSerialized]
        public PICAFixedAttribute[] FixedAttributes;

        [NonSerialized]
        public Vector4D PositionOffset;

        public H3DMesh()
        {
            SubMeshes = new List<H3DSubMesh>();
        }

        public H3DMesh(IEnumerable<PICAVertex> Vertices, PICAAttribute[] Attributes, ushort[] Indices)
        {
            this.Attributes = Attributes;

            RawBuffer = VerticesConverter.GetBuffer(Vertices, Attributes);

            VertexStride = 0;

            foreach (PICAAttribute Attrib in Attributes)
            {
                int Length = Attrib.Elements;

                switch (Attrib.Format)
                {
                    case PICAAttributeFormat.Short: Length <<= 1; break;
                    case PICAAttributeFormat.Float: Length <<= 2; break;
                }

                VertexStride += Length;
            }

            SubMeshes = new List<H3DSubMesh> { new H3DSubMesh { Indices = Indices } };
        }

        public PICAVertex[] ToVertices()
        {
            return VerticesConverter.GetVertices(RawBuffer, VertexStride, Attributes);
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(EnableCommands);

            uint BufferAddress = 0;
            ulong BufferFormats = 0;
            ulong BufferAttributes = 0;
            ulong BufferPermutation = 0;
            int AttributesCount = 0;
            int AttributesTotal = 0;

            int UniformIndex = 0;
            int FixedIndex = 0;

            Vector4D[] Uniform = new Vector4D[96];

            PICAVectorFloat24[] Fixed = new PICAVectorFloat24[12];

            Uniform[6] = new Vector4D(0, 0, 0, 0);
            Uniform[7] = new Vector4D(1, 1, 1, 1);
            Uniform[8] = new Vector4D(1, 1, 1, 1);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                ulong Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_LOW: BufferFormats |= Param; break;
                    case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_HIGH: BufferFormats |= Param << 32; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_OFFSET: BufferAddress = (uint)Param; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG1: BufferAttributes |= Param; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG2:
                        BufferAttributes |= (Param & 0xffff) << 32;
                        VertexStride = (byte)(Param >> 16);
                        AttributesCount = (int)(Param >> 28);
                        break;
                    case PICARegister.GPUREG_FIXEDATTRIB_INDEX: FixedIndex = (int)Param; break;
                    case PICARegister.GPUREG_FIXEDATTRIB_DATA0: Fixed[FixedIndex].Word0 = (uint)Param; break;
                    case PICARegister.GPUREG_FIXEDATTRIB_DATA1: Fixed[FixedIndex].Word1 = (uint)Param; break;
                    case PICARegister.GPUREG_FIXEDATTRIB_DATA2: Fixed[FixedIndex].Word2 = (uint)Param; break;
                    case PICARegister.GPUREG_VSH_NUM_ATTR: AttributesTotal = (int)(Param + 1); break;
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
                        for (int i = 0; i < Cmd.Parameters.Length; i++)
                        {
                            int j = UniformIndex >> 2;
                            int k = (UniformIndex++ & 3) ^ 3;

                            Uniform[j][k] = IOUtils.ToSingle(Cmd.Parameters[i]);
                        }
                        break;
                }
            }

            Attributes = new PICAAttribute[AttributesCount];

            FixedAttributes = new PICAFixedAttribute[AttributesTotal - AttributesCount];

            for (int Index = 0; Index < AttributesTotal; Index++)
            {
                if (((BufferFormats >> (48 + Index)) & 1) != 0)
                {
                    FixedAttributes[Index - AttributesCount] = new PICAFixedAttribute
                    {
                        Name = (PICAAttributeName)((BufferPermutation >> Index * 4) & 0xf),
                        Value = Fixed[Index]
                    };
                }
                else
                {
                    int PermutationIdx = (int)((BufferAttributes >> Index * 4) & 0xf);
                    int AttributeName = (int)((BufferPermutation >> PermutationIdx * 4) & 0xf);
                    int AttributeFmt = (int)((BufferFormats >> PermutationIdx * 4) & 0xf);

                    PICAAttribute Attrib = new PICAAttribute
                    {
                        Name = (PICAAttributeName)AttributeName,
                        Format = (PICAAttributeFormat)(AttributeFmt & 3),
                        Elements = (AttributeFmt >> 2) + 1,
                        Scale = 1
                    };

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

            Deserializer.BaseStream.Seek(BufferAddress, SeekOrigin.Begin);

            RawBuffer = Deserializer.Reader.ReadBytes(BufferCount * VertexStride);

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //Setup flags
            bool UVMap0 = Attributes.Any(x => x.Name == PICAAttributeName.TextureCoordinate0);
            bool UVMap1 = Attributes.Any(x => x.Name == PICAAttributeName.TextureCoordinate1);
            bool UVMap2 = Attributes.Any(x => x.Name == PICAAttributeName.TextureCoordinate2);

            foreach (H3DSubMesh SM in SubMeshes)
            {
                SM.BoolUniforms = BitUtils.SetBit(SM.BoolUniforms, SM.Skinning == H3DSubMeshSkinning.Smooth, 1);
                SM.BoolUniforms = BitUtils.SetBit(SM.BoolUniforms, SM.Skinning == H3DSubMeshSkinning.Rigid, 2);
                SM.BoolUniforms = BitUtils.SetBit(SM.BoolUniforms, UVMap0, 9);
                SM.BoolUniforms = BitUtils.SetBit(SM.BoolUniforms, UVMap1, 10);
                SM.BoolUniforms = BitUtils.SetBit(SM.BoolUniforms, UVMap2, 11);
                SM.BoolUniforms = BitUtils.SetBit(SM.BoolUniforms, UVMap0, 13);
                SM.BoolUniforms = BitUtils.SetBit(SM.BoolUniforms, true, 15);
            }

            //Fill Commands
            PICACommandWriter Writer;

            ulong BufferFormats = 0;
            ulong BufferAttributes = 0;
            ulong BufferPermutation = 0;
            int AttributesTotal = 0;

            float[] Scales = new float[] { 1, 0, 0, 0, 1, 0, 0, 0 };

            //Normal Attributes
            for (int Index = 0; Index < Attributes.Length; Index++)
            {
                PICAAttribute Attrib = Attributes[Index];

                int Shift = AttributesTotal++ * 4;

                ulong AttributeFmt;

                AttributeFmt = (ulong)Attrib.Format;
                AttributeFmt |= (ulong)((Attrib.Elements - 1) & 3) << 2;

                BufferFormats |= AttributeFmt << Shift;
                BufferPermutation |= (ulong)Attrib.Name << Shift;
                BufferAttributes |= (ulong)Index << Shift;

                switch (Attrib.Name)
                {
                    case PICAAttributeName.Position: Scales[3] = Attrib.Scale; break;
                    case PICAAttributeName.Normal: Scales[2] = Attrib.Scale; break;
                    case PICAAttributeName.Tangent: Scales[1] = Attrib.Scale; break;
                    case PICAAttributeName.Color: Scales[0] = Attrib.Scale; break;
                    case PICAAttributeName.TextureCoordinate0: Scales[7] =  Attrib.Scale; break;
                    case PICAAttributeName.TextureCoordinate1: Scales[6] = Attrib.Scale; break;
                    case PICAAttributeName.TextureCoordinate2: Scales[5] = Attrib.Scale; break;
                    case PICAAttributeName.BoneWeight: Scales[4] = Attrib.Scale; break;
                }
            }

            BufferAttributes |= (ulong)(VertexStride & 0xff) << 48;
            BufferAttributes |= (ulong)Attributes.Length << 60;

            //Fixed Attributes
            for (int Index = 0; Index < (FixedAttributes?.Length ?? 0); Index++)
            {
                PICAFixedAttribute Attrib = FixedAttributes[Index];

                BufferFormats |= 1ul << (48 + AttributesTotal); 
                BufferPermutation |= (ulong)Attrib.Name << AttributesTotal++ * 4;
            }

            BufferFormats |= (ulong)(AttributesTotal - 1) << 60;

            Writer = new PICACommandWriter();

            Writer.SetCommand(PICARegister.GPUREG_VSH_INPUTBUFFER_CONFIG, 0xa0000000u | (uint)(AttributesTotal - 1), 0xb);
            Writer.SetCommand(PICARegister.GPUREG_VSH_NUM_ATTR, (uint)(AttributesTotal - 1), 1);
            Writer.SetCommand(PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_LOW, (uint)BufferPermutation);
            Writer.SetCommand(PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_HIGH, (uint)(BufferPermutation >> 32));
            Writer.SetCommand(PICARegister.GPUREG_ATTRIBBUFFERS_LOC, true,
                0, //Base Address (Place holder)
                (uint)BufferFormats,
                (uint)(BufferFormats >> 32),
                0, //Attributes Buffer Address (Place holder)
                (uint)BufferAttributes,
                (uint)(BufferAttributes >> 32));

            for (int Index = 0; Index < (FixedAttributes?.Length ?? 0); Index++)
            {
                PICAFixedAttribute Attrib = FixedAttributes[Index];

                Writer.SetCommand(PICARegister.GPUREG_FIXEDATTRIB_INDEX, true,
                    (uint)(Attributes.Length + Index),
                    Attrib.Value.Word0,
                    Attrib.Value.Word1,
                    Attrib.Value.Word2);
            }

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, true, 0x80000006u,
                    IOUtils.ToUInt32(PositionOffset.W),
                    IOUtils.ToUInt32(PositionOffset.Z),
                    IOUtils.ToUInt32(PositionOffset.Y),
                    IOUtils.ToUInt32(PositionOffset.X));

            //Scales
            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, 0x80000007u);

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA0, false, Scales);

            Writer.WriteEnd();

            EnableCommands = Writer.GetBuffer();

            Writer = new PICACommandWriter();

            //Assuming that the Position isn't used as Fixed Attribute since this doesn't make sense
            Writer.SetCommand(PICARegister.GPUREG_ATTRIBBUFFER0_OFFSET, true, 0, 0, 0);

            for (int Index = 1; Index < 12; Index++)
            {
                Writer.SetCommand(PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG2 + Index * 3, 0);

                if (FixedAttributes?.Any(x => (int)x.Name == Index) ?? false)
                {
                    Writer.SetCommand(PICARegister.GPUREG_FIXEDATTRIB_INDEX, true, (uint)Index, 0, 0, 0);
                }
            }

            Writer.WriteEnd();

            DisableCommands = Writer.GetBuffer();

            return false;
        }

        void ICustomSerializeCmd.SerializeCmd(BinarySerializer Serializer, object Value)
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
