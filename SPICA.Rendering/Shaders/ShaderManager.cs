using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SPICA.Rendering.SPICA_GL;
using System;

namespace SPICA.Rendering.Shaders
{
    public class ShaderManager
    {
        public int Handle { get; private set; }

        private int FragShaderHandle;
        private int VtxShaderHandle;
        private int GeoShaderHandle;

        public ShaderManager() { }

        public ShaderManager(
            int FragShaderHandle,
            int VtxShaderHandle,
            int GeoShaderHandle = 0)
        {
            this.FragShaderHandle = FragShaderHandle;
            this.VtxShaderHandle  = VtxShaderHandle;
            this.GeoShaderHandle  = GeoShaderHandle;

            Link();
        }

        private void Link()
        {
            if ((FragShaderHandle | VtxShaderHandle) != 0)
            {
                Handle = GL.CreateProgram();

                GL.AttachShader(Handle, FragShaderHandle);
                GL.AttachShader(Handle, VtxShaderHandle);

                if (GeoShaderHandle != 0)
                {
                    GL.AttachShader(Handle, GeoShaderHandle);
                }

                GL.LinkProgram(Handle);

                CheckProgramLink(Handle);
            }
        }

        public static void CompileAndCheck(int Handle, string Code)
        {
            GL.ShaderSource(Handle, Code);
            GL.CompileShader(Handle);

            CheckCompilation(Handle);
        }

        public static void CheckCompilation(int Handle)
        {
            int Status = 0;

            GL.GetShader(Handle, ShaderParameter.CompileStatus, out Status);

            if (Status == 0)
            {
                throw new ShaderCompilationException(
                    "Error compiling Shader!" + Environment.NewLine +
                    GL.GetShaderInfoLog(Handle));
            }
        }

        public static void CheckProgramLink(int Handle)
        {
            int Status = 0;

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out Status);

            if (Status == 0)
            {
                throw new ShaderCompilationException(
                    "Error linking Shader!" + Environment.NewLine +
                    GL.GetProgramInfoLog(Handle));
            }
        }

        public void SetBoolean(string UniformName, bool Value)
        {
            GL.Uniform1(GL.GetUniformLocation(Handle, UniformName), Value ? 1 : 0);
        }

        public void SetInteger(string UniformName, int Value)
        {
            GL.Uniform1(GL.GetUniformLocation(Handle, UniformName), Value);
        }

        public void SetVector4(string UniformName, Vector4 Vector)
        {
            GL.Uniform4(GL.GetUniformLocation(Handle, UniformName), ref Vector);
        }

        public void SetVector4(string UniformName, System.Numerics.Vector4 Vector)
        {
            GL.Uniform4(GL.GetUniformLocation(Handle, UniformName), Vector.ToVector4());
        }

        public void SetColor4(string UniformName, Color4 Color)
        {
            GL.Uniform4(GL.GetUniformLocation(Handle, UniformName), Color);
        }

        public void Set2x4Array(string UniformName, ref Matrix4 Matrix, int Index = 0)
        {
            SetVector4($"{UniformName}[{Index * 2 + 0}]", Matrix.Column0);
            SetVector4($"{UniformName}[{Index * 2 + 1}]", Matrix.Column1);
        }

        public void Set3x4Array(string UniformName, ref Matrix4 Matrix, int Index = 0)
        {
            SetVector4($"{UniformName}[{Index * 3 + 0}]", Matrix.Column0);
            SetVector4($"{UniformName}[{Index * 3 + 1}]", Matrix.Column1);
            SetVector4($"{UniformName}[{Index * 3 + 2}]", Matrix.Column2);
        }

        public void Set4x4Array(string UniformName, ref Matrix4 Matrix, int Index = 0)
        {
            SetVector4($"{UniformName}[{Index * 4 + 0}]", Matrix.Column0);
            SetVector4($"{UniformName}[{Index * 4 + 1}]", Matrix.Column1);
            SetVector4($"{UniformName}[{Index * 4 + 2}]", Matrix.Column2);
            SetVector4($"{UniformName}[{Index * 4 + 3}]", Matrix.Column3);
        }

        public void DeleteFragmentShader()
        {
            GL.DeleteShader(FragShaderHandle);
        }

        public void DeleteVertexShader()
        {
            GL.DeleteShader(VtxShaderHandle);
        }

        public void DeleteGeometryShader()
        {
            GL.DeleteShader(GeoShaderHandle);
        }

        public void DeleteAllShaders()
        {
            DeleteFragmentShader();
            DeleteVertexShader();
            DeleteGeometryShader();
        }

        public void DetachAllShaders()
        {
            GL.DetachShader(Handle, FragShaderHandle);
            GL.DetachShader(Handle, VtxShaderHandle);

            if (GeoShaderHandle != 0)
            {
                GL.DetachShader(Handle, GeoShaderHandle);
            }
        }
    }
}
