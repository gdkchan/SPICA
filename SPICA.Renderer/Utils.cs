using OpenTK;
using OpenTK.Graphics.ES30;

using System;

namespace SPICA.Renderer
{
    static class Utils
    {
        public static Tuple<int, int> CreateQuad(Vector2 Position, Vector2 Size)
        {
            float SX = Position.X;
            float SY = Position.Y;
            float EX = SX + Size.X;
            float EY = SY + Size.Y;

            Vector4[] Buffer = new Vector4[]
            {
                new Vector4(SX, SY, 0, 0),
                new Vector4(SX, EY, 0, 1),
                new Vector4(EX, SY, 1, 0),
                new Vector4(EX, EY, 1, 1)
            };

            int VBOHandle = GL.GenBuffer();
            int VAOHandle = GL.GenVertexArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(4 * 16), Buffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 16, 0);

            GL.BindVertexArray(0);

            return Tuple.Create(VBOHandle, VAOHandle);
        }

        public static Matrix4 EulerRotate(Vector3 Rotation)
        {
            Matrix4 RotXMtx = Matrix4.CreateRotationX(Rotation.X);
            Matrix4 RotYMtx = Matrix4.CreateRotationY(Rotation.Y);
            Matrix4 RotZMtx = Matrix4.CreateRotationZ(Rotation.Z);

            return RotXMtx * RotYMtx * RotZMtx;
        }

        public static void SetState(EnableCap Cap, bool Value)
        {
            if (Value)
                GL.Enable(Cap);
            else
                GL.Disable(Cap);
        }
    }
}
