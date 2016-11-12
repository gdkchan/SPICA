using SPICA.PICA.Commands;

namespace SPICA.Formats.GFL2.Model.Mesh
{
    class GFSubMesh
    {
        public byte[] RawBuffer;
        public int VertexStride;
        public PICAAttribute[] Attributes;
        public PICAFixedAttribute[] FixedAttributes;

        public ushort[] Indices;

        public byte[] BoneIndices;
        public byte BoneIndicesCount;

        public uint VerticesCount;
        public uint IndicesCount;
        public uint VerticesLength;
        public uint IndicesLength;

        public uint Hash;
        public string Name;
    }
}
