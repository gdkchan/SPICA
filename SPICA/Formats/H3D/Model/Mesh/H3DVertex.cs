using SPICA.Math3D;

namespace SPICA.Formats.H3D.Model.Mesh
{
    class H3DVertex
    {
        public Vector3D Position;

        public Vector3D Normal;

        public Vector3D Tangent;

        public RGBAFloat Color;

        public Vector2D TextureCoord0;
        public Vector2D TextureCoord1;
        public Vector2D TextureCoord2;

        public int[] Indices;
        public float[] Weights;
        
        public H3DVertex()
        {
            Indices = new int[4];
            Weights = new float[4];
        }
    }
}
