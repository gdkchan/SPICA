using SPICA.Formats.Common;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SPICA.Formats.GFL2.Model.Mesh
{
    public class GFMesh : INamed
    {
        private const string MagicStr = "mesh";

        private static float[] Scales =
        {
            1f / sbyte.MaxValue,
            1f / byte.MaxValue,
            1f / short.MaxValue,
            1
        };

        public string Name { get; set; }

        public Vector4 BBoxMinVector;
        public Vector4 BBoxMaxVector;

        public int BoneIndicesPerVertex;

        public List<GFSubMesh> SubMeshes;

        private struct SubMeshSize
        {
            public int IndicesCount;
            public int IndicesLength;
            public int VerticesCount;
            public int VerticesLength;
        }

        public GFMesh()
        {
            SubMeshes = new List<GFSubMesh>();
        }

        public GFMesh(BinaryReader Reader) : this()
        {
            GFSection MeshSection = new GFSection(Reader);

            long Position = Reader.BaseStream.Position;

            uint   NameHash = Reader.ReadUInt32();
            string NameStr  = Reader.ReadPaddedString(0x40);

            Name = NameStr;

            Reader.ReadUInt32();

            BBoxMinVector = Reader.ReadVector4(); //Is it right? seems to be 0, 0, 0, 1
            BBoxMaxVector = Reader.ReadVector4(); //Is it right? seems to be 0, 0, 0, 1

            uint SubMeshesCount = Reader.ReadUInt32();

            BoneIndicesPerVertex = Reader.ReadInt32();

            Reader.BaseStream.Seek(0x10, SeekOrigin.Current); //Padding

            List<uint[]> CmdList = new List<uint[]>();

            uint CommandsLength;
            uint CommandIndex;
            uint CommandsCount;
            uint Padding;

            do
            {
                CommandsLength = Reader.ReadUInt32();
                CommandIndex   = Reader.ReadUInt32();
                CommandsCount  = Reader.ReadUInt32();
                Padding        = Reader.ReadUInt32();

                uint[] Commands = new uint[CommandsLength >> 2];

                for (int Index = 0; Index < Commands.Length; Index++)
                {
                    Commands[Index] = Reader.ReadUInt32();
                }

                CmdList.Add(Commands);                
            }
            while (CommandIndex < CommandsCount - 1);

            SubMeshSize[] SMSizes = new SubMeshSize[SubMeshesCount];

            //Add SubMesh with Hash, Name and Bone Indices.
            //The rest is added latter (because the data is split inside the file).
            for (int MeshIndex = 0; MeshIndex < SubMeshesCount; MeshIndex++)
            {
                GFSubMesh SM = new GFSubMesh();

                uint SMNameHash = Reader.ReadUInt32();

                SM.Name             = Reader.ReadIntLengthString();
                SM.BoneIndicesCount = Reader.ReadByte();

                for (int Bone = 0; Bone < 0x1f; Bone++)
                {
                    SM.BoneIndices[Bone] = Reader.ReadByte();
                }

                SMSizes[MeshIndex] = new SubMeshSize()
                {
                    VerticesCount  = Reader.ReadInt32(),
                    IndicesCount   = Reader.ReadInt32(),
                    VerticesLength = Reader.ReadInt32(),
                    IndicesLength  = Reader.ReadInt32()
                };

                SubMeshes.Add(SM);
            }

            for (int MeshIndex = 0; MeshIndex < SubMeshesCount; MeshIndex++)
            {
                GFSubMesh SM = SubMeshes[MeshIndex];

                uint[] EnableCommands  = CmdList[MeshIndex * 3 + 0];
                uint[] DisableCommands = CmdList[MeshIndex * 3 + 1];
                uint[] IndexCommands   = CmdList[MeshIndex * 3 + 2];

                PICACommandReader CmdReader;

                CmdReader = new PICACommandReader(EnableCommands);

                PICAVectorFloat24[] Fixed = new PICAVectorFloat24[12];

                ulong BufferFormats     = 0;
                ulong BufferAttributes  = 0;
                ulong BufferPermutation = 0;
                int   AttributesCount   = 0;
                int   AttributesTotal   = 0;

                int FixedIndex = 0;

                while (CmdReader.HasCommand)
                {
                    PICACommand Cmd = CmdReader.GetCommand();

                    uint Param = Cmd.Parameters[0];

                    switch (Cmd.Register)
                    {
                        case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_LOW:  BufferFormats |= (ulong)Param <<  0; break;
                        case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_HIGH: BufferFormats |= (ulong)Param << 32; break;

                        case PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG1:
                            BufferAttributes |= Param;
                            break;

                        case PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG2:
                            BufferAttributes |= (ulong)(Param & 0xffff) << 32;
                            SM.VertexStride   =  (byte)(Param >> 16);
                            AttributesCount   =   (int)(Param >> 28);
                            break;

                        case PICARegister.GPUREG_FIXEDATTRIB_INDEX: FixedIndex = (int)Param; break;

                        case PICARegister.GPUREG_FIXEDATTRIB_DATA0: Fixed[FixedIndex].Word0 = Param; break;
                        case PICARegister.GPUREG_FIXEDATTRIB_DATA1: Fixed[FixedIndex].Word1 = Param; break;
                        case PICARegister.GPUREG_FIXEDATTRIB_DATA2: Fixed[FixedIndex].Word2 = Param; break;

                        case PICARegister.GPUREG_VSH_NUM_ATTR: AttributesTotal = (int)(Param + 1); break;

                        case PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_LOW:  BufferPermutation |= (ulong)Param <<  0; break;
                        case PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_HIGH: BufferPermutation |= (ulong)Param << 32; break;
                    }
                }

                for (int Index = 0; Index < AttributesTotal; Index++)
                {
                    if (((BufferFormats >> (48 + Index)) & 1) != 0)
                    {
                        PICAAttributeName Name = (PICAAttributeName)((BufferPermutation >> Index * 4) & 0xf);

                        float Scale =
                            Name == PICAAttributeName.Color ||
                            Name == PICAAttributeName.BoneWeight ? Scales[1] : 1;

                        SM.FixedAttributes.Add(new PICAFixedAttribute()
                        {
                            Name  = Name,
                            Value = Fixed[Index] * Scale
                        });
                    }
                    else
                    {
                        int PermutationIdx = (int)((BufferAttributes  >> Index          * 4) & 0xf);
                        int AttributeName  = (int)((BufferPermutation >> PermutationIdx * 4) & 0xf);
                        int AttributeFmt   = (int)((BufferFormats     >> PermutationIdx * 4) & 0xf);

                        PICAAttribute Attrib = new PICAAttribute()
                        {
                            Name     = (PICAAttributeName)AttributeName,
                            Format   = (PICAAttributeFormat)(AttributeFmt & 3),
                            Elements = (AttributeFmt >> 2) + 1,
                            Scale    = Scales[AttributeFmt & 3]
                        };

                        if (Attrib.Name == PICAAttributeName.BoneIndex) Attrib.Scale = 1;

                        SM.Attributes.Add(Attrib);
                    }
                }

                CmdReader = new PICACommandReader(IndexCommands);

                uint BufferAddress = 0;
                uint BufferCount   = 0;

                while (CmdReader.HasCommand)
                {
                    PICACommand Cmd = CmdReader.GetCommand();

                    uint Param = Cmd.Parameters[0];

                    switch (Cmd.Register)
                    {
                        case PICARegister.GPUREG_INDEXBUFFER_CONFIG: BufferAddress = Param; break;
                        case PICARegister.GPUREG_NUMVERTICES:        BufferCount   = Param; break;
                        case PICARegister.GPUREG_PRIMITIVE_CONFIG:
                            SM.PrimitiveMode = (PICAPrimitiveMode)(Param >> 8);
                            break;
                    }
                }

                SM.RawBuffer = Reader.ReadBytes(SMSizes[MeshIndex].VerticesLength);

                SM.Indices = new ushort[BufferCount];

                long IndexAddress = Reader.BaseStream.Position;

                for (int Index = 0; Index < BufferCount; Index++)
                {
                    if ((BufferAddress >> 31) != 0)
                        SM.Indices[Index] = Reader.ReadUInt16();
                    else
                        SM.Indices[Index] = Reader.ReadByte();
                }

                Reader.BaseStream.Seek(IndexAddress + SMSizes[MeshIndex].IndicesLength, SeekOrigin.Begin);
            }

            Reader.BaseStream.Seek(Position + MeshSection.Length, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter Writer)
        {
            long StartPosition = Writer.BaseStream.Position;

            new GFSection(MagicStr).Write(Writer);

            GFNV1 FNV = new GFNV1();

            FNV.Hash(Name);

            Writer.Write(FNV.HashCode);
            Writer.WritePaddedString(Name, 0x40);

            Writer.Write(0u);

            Writer.Write(BBoxMinVector);
            Writer.Write(BBoxMaxVector);

            Writer.Write((uint)SubMeshes.Count);

            Writer.Write(BoneIndicesPerVertex);

            Writer.Write(0xfffffffffffffffful);
            Writer.Write(0xfffffffffffffffful);

            for (int Index = 0; Index < SubMeshes.Count; Index++)
            {
                GFSubMesh SM = SubMeshes[Index];

                PICACommandWriter CmdWriter;

                /* Enable commands */
                CmdWriter = new PICACommandWriter();

                ulong BufferFormats     = 0;
                ulong BufferAttributes  = 0;
                ulong BufferPermutation = 0;
                int   AttributesTotal   = 0;

                //Normal Attributes
                for (int Attr = 0; Attr < SM.Attributes.Count; Attr++)
                {
                    PICAAttribute Attrib = SM.Attributes[Attr];

                    int Shift = AttributesTotal++ * 4;

                    ulong AttribFmt;

                    AttribFmt  = (ulong)Attrib.Format;
                    AttribFmt |= (ulong)((Attrib.Elements - 1) & 3) << 2;

                    BufferFormats     |=        AttribFmt   << Shift;
                    BufferPermutation |= (ulong)Attrib.Name << Shift;
                    BufferAttributes  |= (ulong)Attr        << Shift;
                }

                BufferAttributes |= (ulong)(SM.VertexStride & 0xff) << 48;
                BufferAttributes |= (ulong)SM.Attributes.Count << 60;

                //Fixed Attributes
                foreach (PICAFixedAttribute Attrib in SM.FixedAttributes)
                {
                    BufferFormats |= 1ul << (48 + AttributesTotal);

                    BufferPermutation |= (ulong)Attrib.Name << AttributesTotal++ * 4;
                }

                BufferFormats |= (ulong)(AttributesTotal - 1) << 60;

                CmdWriter.SetCommand(PICARegister.GPUREG_VSH_INPUTBUFFER_CONFIG, 0xa0000000u | (uint)(AttributesTotal - 1), 0xb);

                CmdWriter.SetCommand(PICARegister.GPUREG_VSH_NUM_ATTR, (uint)(AttributesTotal - 1), 1);

                CmdWriter.SetCommand(PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_LOW,  (uint)(BufferPermutation >>  0));
                CmdWriter.SetCommand(PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_HIGH, (uint)(BufferPermutation >> 32));

                CmdWriter.SetCommand(PICARegister.GPUREG_ATTRIBBUFFERS_LOC, true,
                    0x03000000u, //Base Address (Place holder)
                    (uint)(BufferFormats >>  0),
                    (uint)(BufferFormats >> 32),
                    0x99999999u, //Attributes Buffer Address (Place holder)
                    (uint)(BufferAttributes >>  0),
                    (uint)(BufferAttributes >> 32));

                for (int Attr = 0; Attr < SM.FixedAttributes.Count; Attr++)
                {
                    PICAFixedAttribute Attrib = SM.FixedAttributes[Attr];

                    CmdWriter.SetCommand(PICARegister.GPUREG_FIXEDATTRIB_INDEX, true,
                        (uint)(SM.Attributes.Count + Attr),
                        Attrib.Value.Word0,
                        Attrib.Value.Word1,
                        Attrib.Value.Word2);
                }

                CmdWriter.WriteEnd();

                WriteCmdsBuff(Writer, CmdWriter.GetBuffer(), Index * 3);

                /* Disable commands */
                CmdWriter = new PICACommandWriter();

                //Assuming that the Position isn't used as Fixed Attribute since this doesn't make sense.
                CmdWriter.SetCommand(PICARegister.GPUREG_ATTRIBBUFFER0_OFFSET, true, 0, 0, 0);

                for (int Attr = 1; Attr < 12; Attr++)
                {
                    CmdWriter.SetCommand(PICARegister.GPUREG_ATTRIBBUFFER0_CONFIG2 + Attr * 3, 0);

                    if (SM.FixedAttributes?.Any(x => (int)x.Name == Attr) ?? false)
                    {
                        CmdWriter.SetCommand(PICARegister.GPUREG_FIXEDATTRIB_INDEX, true, (uint)Attr, 0, 0, 0);
                    }
                }

                CmdWriter.WriteEnd();

                WriteCmdsBuff(Writer, CmdWriter.GetBuffer(), Index * 3 + 1);

                /* Index commands */
                CmdWriter = new PICACommandWriter();

                uint IdxFmtAddr = SM.IsIdx8Bits
                    ? 0x01999999u
                    : 0x81999999u;

                CmdWriter.SetCommand(PICARegister.GPUREG_RESTART_PRIMITIVE, true);

                CmdWriter.SetCommand(PICARegister.GPUREG_INDEXBUFFER_CONFIG, IdxFmtAddr);

                CmdWriter.SetCommand(PICARegister.GPUREG_NUMVERTICES, (uint)SM.Indices.Length);

                CmdWriter.SetCommand(PICARegister.GPUREG_START_DRAW_FUNC0, false, 1);

                CmdWriter.SetCommand(PICARegister.GPUREG_DRAWELEMENTS, true);

                CmdWriter.SetCommand(PICARegister.GPUREG_START_DRAW_FUNC0, true, 1);

                CmdWriter.SetCommand(PICARegister.GPUREG_VTX_FUNC, true);

                CmdWriter.SetCommand(PICARegister.GPUREG_PRIMITIVE_CONFIG, (uint)SM.PrimitiveMode << 8, 8);
                CmdWriter.SetCommand(PICARegister.GPUREG_PRIMITIVE_CONFIG, (uint)SM.PrimitiveMode << 8, 8);

                CmdWriter.WriteEnd();

                WriteCmdsBuff(Writer, CmdWriter.GetBuffer(), Index * 3 + 2);
            }

            foreach (GFSubMesh SM in SubMeshes)
            {
                FNV = new GFNV1();

                FNV.Hash(SM.Name);

                Writer.Write(FNV.HashCode);
                Writer.Write(GetPaddedLen4(SM.Name.Length));
                Writer.WritePaddedString(SM.Name, GetPaddedLen4(SM.Name.Length));

                Writer.Write(SM.BoneIndicesCount);

                byte[] BoneIndices = new byte[0x1f];

                SM.BoneIndices.CopyTo(BoneIndices, 0);

                Writer.Write(BoneIndices);

                int VerticesCount = 0;

                if (SM.VertexStride != 0)
                {
                    VerticesCount = SM.RawBuffer.Length / SM.VertexStride;
                }

                int VtxBuffLen = SM.RawBuffer.Length;
                int IdxBuffLen = SM.Indices.Length * (SM.IsIdx8Bits ? 1 : 2);

                Writer.Write(VerticesCount);
                Writer.Write(SM.Indices.Length);
                Writer.Write(GetPaddedLen4(VtxBuffLen));
                Writer.Write(GetPaddedLen16(IdxBuffLen));
            }

            foreach (GFSubMesh SM in SubMeshes)
            {
                long Position = Writer.BaseStream.Position;

                Writer.Write(SM.RawBuffer);

                while (((Writer.BaseStream.Position - Position) & 3) != 0)
                {
                    Writer.Write((byte)0);
                }

                Position = Writer.BaseStream.Position;

                foreach (ushort Idx in SM.Indices)
                {
                    if (SM.IsIdx8Bits)
                        Writer.Write((byte)Idx);
                    else
                        Writer.Write(Idx);
                }

                while (((Writer.BaseStream.Position - Position) & 0xf) != 0)
                {
                    Writer.Write((byte)0);
                }
            }

            Writer.Align(0x10, 0);

            Writer.Write(0ul);
            Writer.Write(0ul);

            long EndPosition = Writer.BaseStream.Position;

            Writer.BaseStream.Seek(StartPosition + 8, SeekOrigin.Begin);

            Writer.Write((uint)(EndPosition - StartPosition - 0x10));

            Writer.BaseStream.Seek(EndPosition, SeekOrigin.Begin);
        }

        private void WriteCmdsBuff(BinaryWriter Writer, uint[] Cmds, int Index)
        {
            Writer.Write((uint)Cmds.Length * 4);
            Writer.Write((uint)Index);
            Writer.Write((uint)SubMeshes.Count * 3);
            Writer.Write(0u);

            foreach (uint Cmd in Cmds)
            {
                Writer.Write(Cmd);
            }
        }

        private int GetPaddedLen4(int Length)  => (Length + 0x3) & ~0x3;
        private int GetPaddedLen16(int Length) => (Length + 0xf) & ~0xf;
    }
}
