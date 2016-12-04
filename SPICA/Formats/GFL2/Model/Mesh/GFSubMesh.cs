using SPICA.PICA.Commands;

namespace SPICA.Formats.GFL2.Model.Mesh
{
    public class GFSubMesh
    {
        public uint Hash;
        public string Name;

        public byte[] BoneIndices;
        public byte BoneIndicesCount;

        public uint VerticesCount;
        public uint IndicesCount;
        public uint VerticesLength;
        public uint IndicesLength;

        public ushort[] Indices;

        public byte[] RawBuffer;
        public int VertexStride;
        public PICAAttribute[] Attributes;
        public PICAFixedAttribute[] FixedAttributes;
    }
}
