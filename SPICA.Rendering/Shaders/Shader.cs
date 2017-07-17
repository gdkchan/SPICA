using OpenTK;
using OpenTK.Graphics.OpenGL;

using System;

namespace SPICA.Rendering.Shaders
{
    class Shader
    {
        private enum ShaderType
        {
            Vertex,
            Geometry
        }

        public int Handle { get; private set; }

        private int FragShaderHandle;

        private VertexShader VtxShader;

        public Shader() { }

        public Shader(int FragShaderHandle, VertexShader VtxShader)
        {
            this.FragShaderHandle = FragShaderHandle;

            this.VtxShader = VtxShader;

            Link();
        }

        private void Link()
        {
            Handle = GL.CreateProgram();

            Attach(FragShaderHandle);
            Attach(VtxShader.VtxShaderHandle);
            Attach(VtxShader.GeoShaderHandle);

            GL.LinkProgram(Handle);

            CheckProgramLink(Handle);
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
                throw new ShaderException("Error compiling Shader!" + Environment.NewLine +
                    GL.GetShaderInfoLog(Handle));
            }
        }

        public static void CheckProgramLink(int Handle)
        {
            int Status = 0;

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out Status);

            if (Status == 0)
            {
                throw new ShaderException("Error linking Shader!" + Environment.NewLine +
                    GL.GetProgramInfoLog(Handle));
            }
        }

        //Internal Uniform methods

        private void SetVector4(int Id, ShaderType Type, Vector4 Vector)
        {
            GL.Uniform4(GetLocFromId(Id, Type), ref Vector);
        }

        private void Set2x4Array(int Id, ShaderType Type, Matrix4 Matrix)
        {
            GL.Uniform4(GetLocFromId(Id + 0, Type), Matrix.Column0);
            GL.Uniform4(GetLocFromId(Id + 1, Type), Matrix.Column1);
        }

        private void Set3x4Array(int Id, ShaderType Type, Matrix4 Matrix)
        {
            GL.Uniform4(GetLocFromId(Id + 0, Type), Matrix.Column0);
            GL.Uniform4(GetLocFromId(Id + 1, Type), Matrix.Column1);
            GL.Uniform4(GetLocFromId(Id + 2, Type), Matrix.Column2);
        }

        private void Set4x4Array(int Id, ShaderType Type, Matrix4 Matrix)
        {
            GL.Uniform4(GetLocFromId(Id + 0, Type), Matrix.Column0);
            GL.Uniform4(GetLocFromId(Id + 1, Type), Matrix.Column1);
            GL.Uniform4(GetLocFromId(Id + 2, Type), Matrix.Column2);
            GL.Uniform4(GetLocFromId(Id + 3, Type), Matrix.Column3);
        }

        private int GetLocFromId(int Id, ShaderType Type)
        {
            string Name = Type == ShaderType.Vertex
                ? VtxShader.VtxNames.Vec4Uniforms?[Id]
                : VtxShader.GeoNames.Vec4Uniforms?[Id];

            if (Name != null)
            {
                return GL.GetUniformLocation(Handle, Name);
            }
            else
            {
                return -1;
            }
        }

        //Public Uniform methods facades

        public void SetVtxVector4(int Id, Vector4 Vector)
        {
            SetVector4(Id, ShaderType.Vertex, Vector);
        }

        public void SetVtx2x4Array(int Id, Matrix4 Matrix)
        {
            Set2x4Array(Id, ShaderType.Vertex, Matrix);
        }

        public void SetVtx3x4Array(int Id, Matrix4 Matrix)
        {
            Set3x4Array(Id, ShaderType.Vertex, Matrix);
        }

        public void SetVtx4x4Array(int Id, Matrix4 Matrix)
        {
            Set4x4Array(Id, ShaderType.Vertex, Matrix);
        }

        public void SetGeoVector4(int Id, Vector4 Vector)
        {
            SetVector4(Id, ShaderType.Geometry, Vector);
        }

        public void SetGeo2x4Array(int Id, Matrix4 Matrix)
        {
            Set2x4Array(Id, ShaderType.Geometry, Matrix);
        }

        public void SetGeo3x4Array(int Id, Matrix4 Matrix)
        {
            Set3x4Array(Id, ShaderType.Geometry, Matrix);
        }

        public void SetGeo4x4Array(int Id, Matrix4 Matrix)
        {
            Set4x4Array(Id, ShaderType.Geometry, Matrix);
        }

        public void DeleteFragmentShader()
        {
            GL.DeleteShader(FragShaderHandle);
        }

        public void DeleteProgram()
        {
            GL.DeleteProgram(Handle);
        }

        public void DetachAllShaders()
        {
            Detach(FragShaderHandle);
            Detach(VtxShader.VtxShaderHandle);
            Detach(VtxShader.GeoShaderHandle);
        }

        private void Attach(int ShaderHandle)
        {
            if (ShaderHandle != 0) GL.AttachShader(Handle, ShaderHandle);
        }

        private void Detach(int ShaderHandle)
        {
            if (ShaderHandle != 0) GL.DetachShader(Handle, ShaderHandle);
        }
    }
}
