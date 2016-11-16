using OpenTK;

using SPICA.Math3D;
using SPICA.PICA.Commands;

namespace SPICA.Renderer.SPICA_GL
{
    static class VectorExtensions
    {
        public static Vector2 ToVector2(this Vector2D Vector)
        {
            return new Vector2(Vector.X, Vector.Y);
        }

        public static Vector3 ToVector3(this Vector3D Vector)
        {
            return new Vector3(Vector.X, Vector.Y, Vector.Z);
        }

        public static Vector4 ToVector4(this Vector4D Vector)
        {
            return new Vector4(Vector.X, Vector.Y, Vector.Z, Vector.W);
        }

        public static Vector4 ToVector4(this PICAVectorFloat24 Vector)
        {
            return new Vector4(Vector.X, Vector.Y, Vector.Z, Vector.W);
        }

        public static OpenTK.Quaternion ToQuaternion(this Math3D.Quaternion Quat)
        {
            return new OpenTK.Quaternion(Quat.X, Quat.Y, Quat.Z, Quat.W);
        }
    }
}
