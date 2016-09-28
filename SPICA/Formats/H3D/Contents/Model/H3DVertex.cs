using SPICA.Math;

namespace SPICA.Formats.H3D.Contents.Model
{
    struct H3DVertex
    {
        public Vector3D Position;
        public Vector3D Normal;
        public Vector3D Tangent;
        public Vector2D TextureCoord0;
        public Vector2D TextureCoord1;
        public Vector2D TextureCoord2;
        public float[] Weights;
        public int[] Indices;
    }
}
