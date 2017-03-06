using System.IO;

namespace SPICA.Formats.MTFramework
{
    class MTMesh
    {
        public MTMesh(
            BinaryReader Reader,
            uint VerticesBufferAddress,
            uint IndicesBufferAddress)
        {
            Reader.ReadUInt16();
            ushort VerticesCount = Reader.ReadUInt16();
            Reader.ReadUInt16();
            Reader.ReadUInt16();
            Reader.ReadUInt16();
            byte VertexStride = Reader.ReadByte();
            byte AttributesCount = Reader.ReadByte();
            uint VerticesIndex = Reader.ReadUInt32();
            uint VerticesOffset = Reader.ReadUInt32();
            Reader.ReadUInt32();
            uint IndicesOffset = Reader.ReadUInt32();
            uint IndicesCount = Reader.ReadUInt32();
            Reader.ReadUInt32();
            Reader.ReadUInt16();
            ushort MeshIndex = Reader.ReadUInt16();
            ushort VerticesStartIndex = Reader.ReadUInt16();
            ushort VerticesEndIndex = Reader.ReadUInt16();
            Reader.ReadUInt16();
            Reader.ReadByte();
        }
    }
}
