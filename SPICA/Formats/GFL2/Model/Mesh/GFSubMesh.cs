using SPICA.PICA.Commands;

using System.Collections.Generic;

namespace SPICA.Formats.GFL2.Model.Mesh
{
    public class GFSubMesh
    {
        public string Name;

        public byte BoneIndicesCount;

        public byte[] BoneIndices;

        public int VertexStride;

        public ushort[] Indices;

        //Note: All the models observed when writing the model creation logic uses 16 bits
        //for the indices, even those where the indices are always < 256.
        //You can make this store the indices more efficiently when MaxIndex
        //of the Indices buffer is < 256.
        public bool IsIdx8Bits => false;

        public byte[] RawBuffer;

        public PICAPrimitiveMode PrimitiveMode;

        public readonly List<PICAAttribute>      Attributes;
        public readonly List<PICAFixedAttribute> FixedAttributes;

        public GFSubMesh()
        {
            BoneIndices = new byte[0x1f];

            Attributes      = new List<PICAAttribute>();
            FixedAttributes = new List<PICAFixedAttribute>();            
        }
    }
}
