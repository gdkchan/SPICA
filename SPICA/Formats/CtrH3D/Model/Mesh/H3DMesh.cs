using SPICA.Formats.Common;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SPICA.Formats.CtrH3D.Model.Mesh
{
    [Inline]
    public class H3DMesh : ICustomSerialization, ICustomSerializeCmd
    {
        public ushort MaterialIndex;

        [Padding(2)] private byte Flags;

        public ushort NodeIndex;

        private ushort Key;

        public H3DMeshType Type
        {
            get
            {
                return (H3DMeshType)BitUtils.GetBits(Flags, 0, 2);
            }
            set
            {
                Flags = (byte)BitUtils.SetBits(Flags, (uint)value, 0, 2);
            }
        }

        public H3DMeshSkinning Skinning
        {
            get
            {
                return (H3DMeshSkinning)BitUtils.GetBits(Flags, 2, 2);
            }
            set
            {
                Flags = (byte)BitUtils.SetBits(Flags, (uint)value, 2, 2);
            }
        }

        public int Priority
        {
            get
            {
                return (int)BitUtils.GetBits(Key, 0, 8);
            }
            set
            {
                Key = (ushort)BitUtils.SetBits(Key, value, 0, 8);
            }
        }

        public int Layer
        {
            get
            {
                return (int)BitUtils.GetBits(Key, 8, 2);
            }
            set
            {
                Key = (ushort)BitUtils.SetBits(Key, value, 8, 2);
            }
        }

        private uint[] EnableCommands;

        public List<H3DSubMesh> SubMeshes;

        private uint[] DisableCommands;

        public Vector3 MeshCenter;

        public H3DModel Parent;

        private uint UserDefinedAddress;

        public H3DMetaData MetaData;

        [Ignore] public byte[] RawBuffer;
        [Ignore] public int    VertexStride;

        [Ignore] public readonly List<PICAAttribute>      Attributes;
        [Ignore] public readonly List<PICAFixedAttribute> FixedAttributes;

        [Ignore] public Vector4 PositionOffset;

        public H3DMesh()
        {
            SubMeshes = new List<H3DSubMesh>();

            Attributes      = new List<PICAAttribute>();
            FixedAttributes = new List<PICAFixedAttribute>();
        }

        public H3DMesh(IEnumerable<PICAVertex> Vertices, List<PICAAttribute> Attributes, ushort[] Indices) : this()
        {
            H3DMeshImpl(Vertices, Attributes);

            this.Attributes = Attributes ?? throw Exceptions.GetNullException("Attributes");

            SubMeshes.Add(new H3DSubMesh(Indices));
        }

        public H3DMesh(IEnumerable<PICAVertex> Vertices, List<PICAAttribute> Attributes, List<H3DSubMesh> SubMeshes) : this()
        {
            H3DMeshImpl(Vertices, Attributes);

            this.Attributes = Attributes ?? throw Exceptions.GetNullException("Attributes");

            this.SubMeshes  = SubMeshes;
        }

        public H3DMesh(
            byte[]                   RawBuffer,
            int                      VertexStride,
            List<PICAAttribute>      Attributes, 
            List<PICAFixedAttribute> FixedAttributes,
            List<H3DSubMesh>         SubMeshes)
        {
            this.RawBuffer       = RawBuffer;
            this.VertexStride    = VertexStride;
            this.Attributes      = Attributes      ?? new List<PICAAttribute>();
            this.FixedAttributes = FixedAttributes ?? new List<PICAFixedAttribute>();
            this.SubMeshes       = SubMeshes       ?? new List<H3DSubMesh>();
        }

        private void H3DMeshImpl(IEnumerable<PICAVertex> Vertices, List<PICAAttribute> Attributes)
        {
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
        }

        public void UpdateBoolUniforms()
        {
            bool UVMap0 = Attributes.Any(x => x.Name == PICAAttributeName.TexCoord0);
            bool UVMap1 = Attributes.Any(x => x.Name == PICAAttributeName.TexCoord1);
            bool UVMap2 = Attributes.Any(x => x.Name == PICAAttributeName.TexCoord2);
            bool Weight = Attributes.Any(x => x.Name == PICAAttributeName.BoneWeight);

            foreach (H3DSubMesh SM in SubMeshes)
            {
                SM.BoolUniforms = (ushort)BitUtils.SetBit(SM.BoolUniforms, SM.Skinning == H3DSubMeshSkinning.Smooth, 1);
                SM.BoolUniforms = (ushort)BitUtils.SetBit(SM.BoolUniforms, SM.Skinning == H3DSubMeshSkinning.Rigid, 2);
                SM.BoolUniforms = (ushort)BitUtils.SetBit(SM.BoolUniforms, Weight, 8);
                SM.BoolUniforms = (ushort)BitUtils.SetBit(SM.BoolUniforms, UVMap0, 9);
                SM.BoolUniforms = (ushort)BitUtils.SetBit(SM.BoolUniforms, UVMap1, 10);
                SM.BoolUniforms = (ushort)BitUtils.SetBit(SM.BoolUniforms, UVMap2, 11);
                SM.BoolUniforms = (ushort)BitUtils.SetBit(SM.BoolUniforms, UVMap0, 13);
                SM.BoolUniforms = (ushort)BitUtils.SetBit(SM.BoolUniforms, true, 15);
            }
        }

        public PICAVertex[] ToVertices()
        {
            return VerticesConverter.GetVertices(this);
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(EnableCommands);

            uint  BufferAddress     = 0;
            ulong BufferFormats     = 0;
            ulong BufferAttributes  = 0;
            ulong BufferPermutation = 0;
            int   AttributesCount   = 0;
            int   AttributesTotal   = 0;

            int FixedIndex   = 0;

            PICAVectorFloat24[] Fixed = new PICAVectorFloat24[12];

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                ulong Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_LOW:  BufferFormats |= Param <<  0; break;
                    case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_HIGH: BufferFormats |= Param << 32; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_OFFSET: BufferAddress = (uint)Param; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG1: BufferAttributes |= Param; break;
                    case PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG2:
                        BufferAttributes |= (Param & 0xffff) << 32;
                        VertexStride    = (byte)(Param >> 16);
                        AttributesCount =  (int)(Param >> 28);
                        break;
                    case PICARegister.GPUREG_FIXEDATTRIB_INDEX: FixedIndex = (int)Param; break;
                    case PICARegister.GPUREG_FIXEDATTRIB_DATA0: Fixed[FixedIndex].Word0 = (uint)Param; break;
                    case PICARegister.GPUREG_FIXEDATTRIB_DATA1: Fixed[FixedIndex].Word1 = (uint)Param; break;
                    case PICARegister.GPUREG_FIXEDATTRIB_DATA2: Fixed[FixedIndex].Word2 = (uint)Param; break;
                    case PICARegister.GPUREG_VSH_NUM_ATTR: AttributesTotal = (int)(Param + 1); break;
                    case PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_LOW:  BufferPermutation |= Param <<  0; break;
                    case PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_HIGH: BufferPermutation |= Param << 32; break;
                }
            }

            for (int Index = 0; Index < AttributesTotal; Index++)
            {
                if (((BufferFormats >> (48 + Index)) & 1) != 0)
                {
                    FixedAttributes.Add(new PICAFixedAttribute
                    {
                        Name  = (PICAAttributeName)((BufferPermutation >> Index * 4) & 0xf),
                        Value = Fixed[Index]
                    });
                }
                else
                {
                    int PermutationIdx = (int)((BufferAttributes  >> Index          * 4) & 0xf);
                    int AttributeName  = (int)((BufferPermutation >> PermutationIdx * 4) & 0xf);
                    int AttributeFmt   = (int)((BufferFormats     >> PermutationIdx * 4) & 0xf);

                    PICAAttribute Attrib = new PICAAttribute
                    {
                        Name     = (PICAAttributeName)AttributeName,
                        Format   = (PICAAttributeFormat)(AttributeFmt & 3),
                        Elements = (AttributeFmt >> 2) + 1,
                        Scale    = 1
                    };

                    switch (Attrib.Name)
                    {
                        case PICAAttributeName.Position:   Attrib.Scale = Reader.Uniforms[7].X; break;
                        case PICAAttributeName.Normal:     Attrib.Scale = Reader.Uniforms[7].Y; break;
                        case PICAAttributeName.Tangent:    Attrib.Scale = Reader.Uniforms[7].Z; break;
                        case PICAAttributeName.Color:      Attrib.Scale = Reader.Uniforms[7].W; break;
                        case PICAAttributeName.TexCoord0:  Attrib.Scale = Reader.Uniforms[8].X; break;
                        case PICAAttributeName.TexCoord1:  Attrib.Scale = Reader.Uniforms[8].Y; break;
                        case PICAAttributeName.TexCoord2:  Attrib.Scale = Reader.Uniforms[8].Z; break;
                        case PICAAttributeName.BoneWeight: Attrib.Scale = Reader.Uniforms[8].W; break;
                    }

                    Attributes.Add(Attrib);
                }
            }

            long Position = Deserializer.BaseStream.Position;

            int BufferCount = 0;

            //The PICA doesn't need the total number of Attributes on the Buffer, so it is not present on the Commands
            //So we need to get the Max Index used on the Index Buffer to figure out the total number of Attributes
            foreach (H3DSubMesh SM in SubMeshes)
            {
                if (BufferCount < SM.MaxIndex)
                    BufferCount = SM.MaxIndex;
            }

            BufferCount++;

            PositionOffset = Reader.Uniforms[6];

            Deserializer.BaseStream.Seek(BufferAddress, SeekOrigin.Begin);

            RawBuffer = Deserializer.Reader.ReadBytes(BufferCount * VertexStride);

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //Fill Commands
            PICACommandWriter Writer;

            ulong BufferFormats     = 0;
            ulong BufferAttributes  = 0;
            ulong BufferPermutation = 0;
            int   AttributesTotal   = 0;

            float[] Scales = new float[] { 1, 0, 0, 0, 1, 0, 0, 0 };

            //Normal Attributes
            for (int Index = 0; Index < Attributes.Count; Index++)
            {
                PICAAttribute Attrib = Attributes[Index];

                int Shift = AttributesTotal++ * 4;

                ulong AttributeFmt;

                AttributeFmt  = (ulong)Attrib.Format;
                AttributeFmt |= (ulong)((Attrib.Elements - 1) & 3) << 2;

                BufferFormats     |= AttributeFmt << Shift;
                BufferPermutation |= (ulong)Attrib.Name << Shift;
                BufferAttributes  |= (ulong)Index << Shift;

                switch (Attrib.Name)
                {
                    case PICAAttributeName.Position:   Scales[3] = Attrib.Scale; break;
                    case PICAAttributeName.Normal:     Scales[2] = Attrib.Scale; break;
                    case PICAAttributeName.Tangent:    Scales[1] = Attrib.Scale; break;
                    case PICAAttributeName.Color:      Scales[0] = Attrib.Scale; break;
                    case PICAAttributeName.TexCoord0:  Scales[7] = Attrib.Scale; break;
                    case PICAAttributeName.TexCoord1:  Scales[6] = Attrib.Scale; break;
                    case PICAAttributeName.TexCoord2:  Scales[5] = Attrib.Scale; break;
                    case PICAAttributeName.BoneWeight: Scales[4] = Attrib.Scale; break;
                }
            }

            BufferAttributes |= (ulong)(VertexStride & 0xff) << 48;
            BufferAttributes |= (ulong)Attributes.Count << 60;

            //Fixed Attributes
            for (int Index = 0; Index < FixedAttributes.Count; Index++)
            {
                PICAFixedAttribute Attrib = FixedAttributes[Index];

                BufferFormats     |= 1ul << (48 + AttributesTotal); 
                BufferPermutation |= (ulong)Attrib.Name << AttributesTotal++ * 4;
            }

            BufferFormats |= (ulong)(AttributesTotal - 1) << 60;

            Writer = new PICACommandWriter();

            Writer.SetCommand(PICARegister.GPUREG_VSH_INPUTBUFFER_CONFIG, 0xa0000000u | (uint)(AttributesTotal - 1), 0xb);
            Writer.SetCommand(PICARegister.GPUREG_VSH_NUM_ATTR, (uint)(AttributesTotal - 1), 1);
            Writer.SetCommand(PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_LOW,  (uint)(BufferPermutation >>  0));
            Writer.SetCommand(PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_HIGH, (uint)(BufferPermutation >> 32));
            Writer.SetCommand(PICARegister.GPUREG_ATTRIBBUFFERS_LOC, true,
                0, //Base Address (Place holder)
                (uint)(BufferFormats >>  0),
                (uint)(BufferFormats >> 32),
                0, //Attributes Buffer Address (Place holder)
                (uint)(BufferAttributes >>  0),
                (uint)(BufferAttributes >> 32));

            for (int Index = 0; Index < FixedAttributes.Count; Index++)
            {
                PICAFixedAttribute Attrib = FixedAttributes[Index];

                Writer.SetCommand(PICARegister.GPUREG_FIXEDATTRIB_INDEX, true,
                    (uint)(Attributes.Count + Index),
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
                    Value    = RawBuffer,
                    Position = Position + 0x30
                });

                Serializer.Pointers.Add(Position + 0x20);

                Serializer.Relocator.RelocTypes.Add(Position + 0x20, H3DRelocationType.BaseAddress);
                Serializer.Relocator.RelocTypes.Add(Position + 0x30, H3DRelocationType.RawDataVertex);
            }
        }
    }
}
