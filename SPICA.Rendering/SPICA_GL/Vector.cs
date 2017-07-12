using OpenTK;

using SPICA.PICA.Commands;

namespace SPICA.Rendering.SPICA_GL
{
    static class VectorExtensions
    {
        public static Vector2 ToVector2(this System.Numerics.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 ToVector3(this System.Numerics.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector4 ToVector4(this System.Numerics.Vector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Vector4 ToVector4(this PICAVectorFloat24 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Quaternion ToQuaternion(this System.Numerics.Quaternion q)
        {
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }
    }
}
