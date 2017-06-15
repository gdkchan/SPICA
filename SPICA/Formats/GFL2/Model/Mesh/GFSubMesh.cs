using SPICA.PICA.Commands;

using System.Collections.Generic;

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

        public readonly List<PICAAttribute>      Attributes;
        public readonly List<PICAFixedAttribute> FixedAttributes;

        public GFSubMesh()
        {
            Attributes      = new List<PICAAttribute>();
            FixedAttributes = new List<PICAFixedAttribute>();
        }
    }
}
