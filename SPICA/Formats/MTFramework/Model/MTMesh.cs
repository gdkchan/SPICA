using SPICA.Formats.MTFramework.Shader;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.MTFramework.Model
{
    public class MTMesh
    {
        public ushort[] Indices;

        public byte[] RawBuffer;

        public byte VertexStride;

        public List<PICAAttribute> Attributes;

        public uint  MaterialIndex;
        public uint  MeshGroupIndex;
        public sbyte RenderType;
        public byte  RenderPriority;
        public byte  BoneIndicesIndex;

        public MTMesh(
            BinaryReader Reader,
            MTShaderEffects Shader,
            uint VerticesBufferAddress,
            uint IndicesBufferAddress)
        {
            ushort MeshTypeFlags  = Reader.ReadUInt16(); //?
            ushort VerticesCount  = Reader.ReadUInt16();
            uint MatMeshIndices   = Reader.ReadUInt32();
            byte MeshFlags        = Reader.ReadByte(); //?
            byte RenderPriority   = Reader.ReadByte();
            byte VertexStride     = Reader.ReadByte();
            byte AttributesCount  = Reader.ReadByte();
            uint VerticesIndex    = Reader.ReadUInt32();
            uint VerticesOffset   = Reader.ReadUInt32();
            uint VertexFormatHash = Reader.ReadUInt32();
            uint IndicesIndex     = Reader.ReadUInt32();
            uint IndicesCount     = Reader.ReadUInt32();
            uint IndicesOffset    = Reader.ReadUInt32();
            byte BoneIndicesCount = Reader.ReadByte(); //Always 0? Probably wrong
            byte BoneIndicesIndex = Reader.ReadByte();
            ushort MeshIndex      = Reader.ReadUInt16();

            this.VertexStride     = VertexStride;
            this.RenderPriority   = RenderPriority;
            this.BoneIndicesIndex = BoneIndicesIndex;

            MeshGroupIndex = (MatMeshIndices >>  0) & 0xfff; //?
            MaterialIndex  = (MatMeshIndices >> 12) & 0xfff;
            RenderType = (sbyte)(MatMeshIndices >> 24);

            Attributes = Shader.GetDescriptor<MTAttributesGroup>(VertexFormatHash).Attributes;

            Reader.BaseStream.Seek(IndicesBufferAddress + IndicesIndex * 2, SeekOrigin.Begin);

            Indices = new ushort[(IndicesCount - 1) * 3];

            for (int Index = 0; Index < Indices.Length; Index++)
            {
                //Convert Triangle Strip Index Buffer to Triangle List
                if (Index > 2)
                {
                    Indices[Index + 0] = Indices[Index - 2];
                    Indices[Index + 1] = Indices[Index - 1];

                    Index += 2;
                }

                Indices[Index] = (ushort)(Reader.ReadUInt16() - VerticesIndex);
            }

            VerticesOffset += VerticesIndex * VertexStride;

            Reader.BaseStream.Seek(VerticesBufferAddress + VerticesOffset, SeekOrigin.Begin);

            RawBuffer = Reader.ReadBytes(VerticesCount * VertexStride);
        }
    }
}
