using OpenTK;

namespace SPICA.Renderer.SPICA_GL
{
    static class MatrixExtensions
    {
        public static Matrix4 ToMatrix4(this Math3D.Matrix3x4 Matrix)
        {
            return new Matrix4
            {
                M11 = Matrix.M11,
                M12 = Matrix.M21,
                M13 = Matrix.M31,
                M14 = 0,

                M21 = Matrix.M12,
                M22 = Matrix.M22,
                M23 = Matrix.M32,
                M24 = 0,

                M31 = Matrix.M13,
                M32 = Matrix.M23,
                M33 = Matrix.M33,
                M34 = 0,

                M41 = Matrix.M14,
                M42 = Matrix.M24,
                M43 = Matrix.M34,
                M44 = 1
            };
        }
    }
}
