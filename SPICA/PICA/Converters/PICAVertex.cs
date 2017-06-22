using System.Numerics;

namespace SPICA.PICA.Converters
{
    public struct PICAVertex
    {
        public Vector4     Position;
        public Vector4     Normal;
        public Vector4     Tangent;
        public Vector4     Color;
        public Vector4     TexCoord0;
        public Vector4     TexCoord1;
        public Vector4     TexCoord2;
        public BoneIndices Indices;
        public BoneWeights Weights;
    }
}
