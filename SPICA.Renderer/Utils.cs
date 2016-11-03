using OpenTK;

namespace SPICA.Renderer
{
    class Utils
    {
        public static Matrix4 EulerRotate(Vector3 Rotation)
        {
            Matrix4 RotXMtx = Matrix4.CreateRotationX(Rotation.X);
            Matrix4 RotYMtx = Matrix4.CreateRotationY(Rotation.Y);
            Matrix4 RotZMtx = Matrix4.CreateRotationZ(Rotation.Z);

            return RotXMtx * RotYMtx * RotZMtx;
        }
    }
}
