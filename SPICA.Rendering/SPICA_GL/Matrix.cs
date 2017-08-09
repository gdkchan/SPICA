using OpenTK;

namespace SPICA.Rendering.SPICA_GL
{
    static class MatrixExtensions
    {
        public static Matrix4 ToMatrix4(this Math3D.Matrix3x4 m)
        {
            return new Matrix4(
                m.M11, m.M12, m.M13, 0f,
                m.M21, m.M22, m.M23, 0f,
                m.M31, m.M32, m.M33, 0f,
                m.M41, m.M42, m.M43, 1f);
        }

        public static Matrix4 ToMatrix4(this System.Numerics.Matrix4x4 m)
        {
            return new Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }
    }
}
