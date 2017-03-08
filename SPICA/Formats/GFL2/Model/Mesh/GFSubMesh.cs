using SPICA.PICA.Commands;

namespace SPICA.Formats.GFL2.Model.Mesh
{
    public class GFSubMesh
    {
        public uint Hash;
        public string Name;

        public byte BoneIndicesCount;

        public byte[] BoneIndices;

        public uint VerticesCount;
        public uint IndicesCount;
        public uint VerticesLength;
        public uint IndicesLength;
        public int  VertexStride;

        public ushort[] Indices;

        public byte[] RawBuffer;

        public PICAAttribute[] Attributes;

        public PICAFixedAttribute[] FixedAttributes;
    }
}
