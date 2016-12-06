using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;

using SPICA.Renderer.GUI;

using System;

namespace SPICA.Renderer
{
    public static class RenderUtils
    {
        public static void SetupShaderForPosCol(int ShaderHandle)
        {
            //Set the fragment shader to a state capable of rendering our Vertex Buffer
            //In this case we set all stages to the default values (output = Vertex Color)
            for (int Index = 0; Index < 6; Index++)
            {
                int ColorCombLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].ColorCombine");
                int AlphaCombLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].AlphaCombine");

                int ColorScaleLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].ColorScale");
                int AlphaScaleLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].AlphaScale");

                int UpColorBuffLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].UpColorBuff");
                int UpAlphaBuffLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].UpAlphaBuff");

                GL.Uniform1(ColorCombLocation, 0);
                GL.Uniform1(AlphaCombLocation, 0);

                GL.Uniform1(ColorScaleLocation, 1f);
                GL.Uniform1(AlphaScaleLocation, 1f);

                GL.Uniform1(UpColorBuffLocation, 1);
                GL.Uniform1(UpAlphaBuffLocation, 1);

                for (int Param = 0; Param < 3; Param++)
                {
                    int ColorSrcLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].ColorSrc");
                    int AlphaSrcLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].AlphaSrc");
                    int ColorOpLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].ColorOp");
                    int AlphaOpLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].AlphaOp");

                    GL.Uniform1(ColorSrcLocation, 0);
                    GL.Uniform1(AlphaSrcLocation, 0);
                    GL.Uniform1(ColorOpLocation, 0);
                    GL.Uniform1(AlphaOpLocation, 0);
                }

                int ColorLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Color");

                GL.Uniform4(ColorLocation, new Color4(0, 0, 0, 0));
            }

            int FixedColorLocation = GL.GetUniformLocation(ShaderHandle, "FixedColor");
            int FixedBoneLocation = GL.GetUniformLocation(ShaderHandle, "FixedBone");
            int FixedWeightLocation = GL.GetUniformLocation(ShaderHandle, "FixedWeight");

            GL.Uniform4(FixedColorLocation, new Vector4(-1));
            GL.Uniform4(FixedBoneLocation, new Vector4(0));
            GL.Uniform4(FixedWeightLocation, new Vector4(0));

            GL.Uniform4(GL.GetUniformLocation(ShaderHandle, "PosOffset"), Vector4.Zero);
            GL.Uniform4(GL.GetUniformLocation(ShaderHandle, "Scales0"), Vector4.One);
            GL.Uniform4(GL.GetUniformLocation(ShaderHandle, "Scales1"), Vector4.One);

            Matrix4 RootTransform = Matrix4.Identity;

            int RootTransformLocation = GL.GetUniformLocation(ShaderHandle, "Transforms[0]");

            GL.UniformMatrix4(RootTransformLocation, false, ref RootTransform);

            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "SmoothSkin"), 0);
        }

        public static Tuple<int, int> UploadQuad(Vector2 Position, Vector2 Size, GUIDockMode DockMode)
        {
            int[] Viewport = new int[4];

            GL.GetInteger(GetPName.Viewport, Viewport);

            float ScrnWidth = Viewport[2];
            float ScrnHeight = Viewport[3];

            float PosX = Position.X / ScrnWidth;
            float PosY = Position.Y / ScrnHeight;

            float SzX = Size.X / ScrnWidth;
            float SzY = Size.Y / ScrnHeight;

            float InvX = (1 - PosX) - SzX;
            float InvY = (1 - PosY) - SzY;

            float SX = 0.5f - SzX * 0.5f;
            float SY = 0.5f - SzY * 0.5f;

            if ((DockMode & GUIDockMode.Top)    != 0) SY = InvY;
            if ((DockMode & GUIDockMode.Left)   != 0) SX = PosX;
            if ((DockMode & GUIDockMode.Right)  != 0) SX = InvX;
            if ((DockMode & GUIDockMode.Bottom) != 0) SY = PosY;

            float EX = SX + SzX;
            float EY = SY + SzY;

            Vector4[] Buffer = new Vector4[]
            {
                new Vector4(SX, SY, 0, 1),
                new Vector4(SX, EY, 0, 0),
                new Vector4(EX, SY, 1, 1),
                new Vector4(EX, EY, 1, 0)
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

        public static int Upload2DTexture(IntPtr ImgPtr, int Width, int Height)
        {
            int TextureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget2d.Texture2D,
                0,
                TextureComponentCount.Rgba,
                Width,
                Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                ImgPtr);

            return TextureId;
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
