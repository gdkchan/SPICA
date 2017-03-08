using SPICA.Formats.Utils;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Model.Mesh
{
    public class GFMesh
    {
        private static float[] Scales =
        {
            1f / sbyte.MaxValue,
            1f / byte.MaxValue,
            1f / short.MaxValue,
            1
        };

        public uint Hash;
        public string Name;

        public Vector4D BBoxMinVector;
        public Vector4D BBoxMaxVector;

        public int BoneIndicesPerVertex;

        public List<GFSubMesh> SubMeshes;

        public GFMesh()
        {
            SubMeshes = new List<GFSubMesh>();
        }

        public GFMesh(BinaryReader Reader) : this()
        {
            GFSection MeshSection = new GFSection(Reader);

            long Position = Reader.BaseStream.Position;

            Hash = Reader.ReadUInt32();
            Name = Reader.ReadPaddedString(0x40);

            Reader.ReadUInt32();

            BBoxMinVector = new Vector4D(Reader); //Not sure
            BBoxMaxVector = new Vector4D(Reader); //Not sure

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

            //Add SubMesh with Hash, Name and Bone Indices
            //The rest is added latter (because the data is split inside the file)
            for (int MeshIndex = 0; MeshIndex < SubMeshesCount; MeshIndex++)
            {
                uint   Hash = Reader.ReadUInt32();
                string Name = Reader.ReadIntLengthString();

                byte BoneIndicesCount = Reader.ReadByte();

                byte[] BoneIndices = new byte[0x1f];

                for (int Bone = 0; Bone < BoneIndices.Length; Bone++)
                {
                    BoneIndices[Bone] = Reader.ReadByte();
                }

                SubMeshes.Add(new GFSubMesh
                {
                    BoneIndices      = BoneIndices,
                    BoneIndicesCount = BoneIndicesCount,

                    VerticesCount  = Reader.ReadUInt32(),
                    IndicesCount   = Reader.ReadUInt32(),
                    VerticesLength = Reader.ReadUInt32(),
                    IndicesLength  = Reader.ReadUInt32(),

                    Hash = Hash,
                    Name = Name
                });
            }

            for (int MeshIndex = 0; MeshIndex < SubMeshesCount; MeshIndex++)
            {
                uint[] EnableCommands  = CmdList[MeshIndex * 3 + 0];
                uint[] DisableCommands = CmdList[MeshIndex * 3 + 1];
                uint[] IndexCommands   = CmdList[MeshIndex * 3 + 2];

                PICACommandReader CmdReader;

                CmdReader = new PICACommandReader(EnableCommands);

                PICAVectorFloat24[] Fixed = new PICAVectorFloat24[12];

                ulong BufferFormats = 0;
                ulong BufferAttributes = 0;
                ulong BufferPermutation = 0;
                int AttributesCount = 0;
                int AttributesTotal = 0;
                int VertexStride = 0;

                int FixedIndex = 0;

                while (CmdReader.HasCommand)
                {
                    PICACommand Cmd = CmdReader.GetCommand();

                    ulong Param = Cmd.Parameters[0];

                    switch (Cmd.Register)
                    {
                        case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_LOW:  BufferFormats |= Param <<  0; break;
                        case PICARegister.GPUREG_ATTRIBBUFFERS_FORMAT_HIGH: BufferFormats |= Param << 32; break;
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
                        case PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_LOW:  BufferPermutation |= Param <<  0; break;
                        case PICARegister.GPUREG_VSH_ATTRIBUTES_PERMUTATION_HIGH: BufferPermutation |= Param << 32; break;
                    }
                }

                PICAAttribute[] Attributes = new PICAAttribute[AttributesCount];

                PICAFixedAttribute[] FixedAttributes = new PICAFixedAttribute[AttributesTotal - AttributesCount];

                for (int Index = 0; Index < AttributesTotal; Index++)
                {
                    if (((BufferFormats >> (48 + Index)) & 1) != 0)
                    {
                        PICAAttributeName Name = (PICAAttributeName)((BufferPermutation >> Index * 4) & 0xf);

                        float Scale =
                            Name == PICAAttributeName.Color ||
                            Name == PICAAttributeName.BoneWeight ? Scales[1] : 1;

                        FixedAttributes[Index - AttributesCount] = new PICAFixedAttribute
                        {
                            Name = Name,
                            Value = Fixed[Index] * Scale
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
                            Scale = Scales[AttributeFmt & 3]
                        };

                        if (Attrib.Name == PICAAttributeName.BoneIndex) Attrib.Scale = 1;

                        Attributes[Index] = Attrib;
                    }
                }

                CmdReader = new PICACommandReader(IndexCommands);

                bool IndexFormat = false;
                uint PrimitivesCount = 0;

                while (CmdReader.HasCommand)
                {
                    PICACommand Cmd = CmdReader.GetCommand();

                    uint Param = Cmd.Parameters[0];

                    switch (Cmd.Register)
                    {
                        case PICARegister.GPUREG_INDEXBUFFER_CONFIG: IndexFormat = (Param >> 31) != 0; break;
                        case PICARegister.GPUREG_NUMVERTICES: PrimitivesCount = Param; break;
                    }
                }

                GFSubMesh SM = SubMeshes[MeshIndex];

                SM.RawBuffer       = Reader.ReadBytes((int)SM.VerticesLength);
                SM.VertexStride    = VertexStride;
                SM.Attributes      = Attributes;
                SM.FixedAttributes = FixedAttributes;

                SM.Indices = new ushort[PrimitivesCount];

                long IndexAddress = Reader.BaseStream.Position;

                for (int Index = 0; Index < PrimitivesCount; Index++)
                {
                    if (IndexFormat)
                        SM.Indices[Index] = Reader.ReadUInt16();
                    else
                        SM.Indices[Index] = Reader.ReadByte();
                }

                Reader.BaseStream.Seek(IndexAddress + SM.IndicesLength, SeekOrigin.Begin);
            }

            Reader.BaseStream.Seek(Position + MeshSection.Length, SeekOrigin.Begin);
        }

        public static List<GFMesh> ReadList(BinaryReader Reader, int Count)
        {
            List<GFMesh> Output = new List<GFMesh>();

            for (int Index = 0; Index < Count; Index++)
            {
                Output.Add(new GFMesh(Reader));
            }

            return Output;
        }
    }
}
