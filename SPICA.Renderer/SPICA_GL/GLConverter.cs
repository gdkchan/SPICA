using OpenTK;
using OpenTK.Graphics;

using SPICA.Math3D;
using SPICA.PICA.Commands;

namespace SPICA.Renderer.SPICA_GL
{
    static class GLConverter
    {
        public static Color4 ToColor(RGBA Color)
        {
            return new Color4(Color.R, Color.G, Color.B, Color.A);
        }

        public static Color4 ToColor(RGBAFloat Color)
        {
            return new Color4(Color.R, Color.G, Color.B, Color.A);
        }

        public static Vector2 ToVector2(Vector2D Vector)
        {
            return new Vector2(Vector.X, Vector.Y);
        }

        public static Vector3 ToVector3(Vector3D Vector)
        {
            return new Vector3(Vector.X, Vector.Y, Vector.Z);
        }

        public static Vector4 ToVector4(Vector4D Vector)
        {
            return new Vector4(Vector.X, Vector.Y, Vector.Z, Vector.W);
        }

        public static Vector4 ToVector4(PICAVectorFloat24 Vector)
        {
            return new Vector4(Vector.X, Vector.Y, Vector.Z, Vector.W);
        }

        public static Matrix4 ToMatrix4(Math3D.Matrix3x4 Matrix)
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
