using System;
using System.Collections.Generic;

namespace SPICA.PICA.Shader
{
    public class ShaderProgram
    {
        public readonly ShaderUniformBool[] BoolUniforms;
        public readonly ShaderUniformVec4[] Vec4Uniforms;
        public readonly ShaderUniformVec4[] IVec4Uniforms;

        public readonly string[] InputRegs;

        public readonly ShaderOutputReg[] OutputRegs;

        public readonly List<ShaderLabel> Labels;

        public bool IsGeometryShader;

        public uint MainOffset;
        public uint EndMainOffset;

        public ShaderProgram()
        {
            BoolUniforms  = new ShaderUniformBool[16];
            Vec4Uniforms  = new ShaderUniformVec4[96];
            IVec4Uniforms = new ShaderUniformVec4[4];

            InitArray(BoolUniforms,  "uniform_bool");
            InitArray(Vec4Uniforms,  "uniform_float");
            InitArray(IVec4Uniforms, "uniform_int");

            InputRegs = new string[16];

            for (int i = 0; i < InputRegs.Length; i++)
            {
                InputRegs[i] = $"input_attrib_{i}";
            }

            OutputRegs = new ShaderOutputReg[16];

            Labels = new List<ShaderLabel>();
        }

        private void InitArray<T>(T[] Array, string BaseName) where T : ShaderUniform
        {
            for (int i = 0; i < Array.Length; i++)
            {
                T Uniform = (T)Activator.CreateInstance(typeof(T));

                Uniform.Name = $"{BaseName}_{i}";

                Array[i] = Uniform;
            }
        }
    }
}
