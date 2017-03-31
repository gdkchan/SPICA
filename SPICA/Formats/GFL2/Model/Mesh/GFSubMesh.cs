using SPICA.PICA.Commands;

namespace SPICA.Formats.GFL2.Model.Mesh
{
    public class GFSubMesh
    {
        public uint Hash;
        public string Name;

        public byte BoneIndicesCount;

        public byte[] BoneIndices;
        
        public uint IndicesCount;
        public uint IndicesLength;
        public uint VerticesCount;
        public uint VerticesLength;
        public int  VertexStride;

        public ushort[] Indices;

        public byte[] RawBuffer;

        public PICAAttribute[] Attributes;

        public PICAFixedAttribute[] FixedAttributes;
    }
}
