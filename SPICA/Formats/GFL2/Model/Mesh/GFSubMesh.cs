using SPICA.PICA.Commands;

namespace SPICA.Formats.GFL2.Model.Mesh
{
    struct GFSubMesh
    {
        public byte[] RawBuffer;
        public int VertexStride;
        public PICAAttribute[] Attributes;
        public PICAFixedAttribute[] FixedAttributes;

        public ushort[] Indices;
        public byte[] BoneIndices;
    }
}
