using SPICA.PICA.Shader;

using System;
using System.Text;

namespace SPICA.Rendering.Shaders
{
    class VertexShaderGenerator : ShaderGenerator
    {
        public VertexShaderGenerator(ShaderBinary SHBin) : base(SHBin) { }

        public string GetVtxShader(int ProgramIndex, bool HasGeoShader, out ShaderNameBlock Names)
        {
            ShaderProgram Program = SHBin.Programs[ProgramIndex];

            Initialize(Program);

            StringBuilder Output = new StringBuilder();

            Output.AppendLine("//SPICA auto-generated code");
            Output.AppendLine("//This code was translated from a MAESTRO Vertex Shader");
            Output.AppendLine("#version 330 core");
            Output.AppendLine("precision highp float;");

            Output.AppendLine();

            GenVec4Uniforms(Output, Program.Vec4Uniforms, Vec4Type);
            GenVec4Uniforms(Output, Program.IVec4Uniforms, IVec4Type);

            Output.AppendLine($"uniform int {BoolsName};");

            Output.AppendLine();

            for (int i = 0; i < Program.BoolUniforms.Length; i++)
            {
                string Name = Program.BoolUniforms[i]?.Name;

                if (Name != null)
                {
                    Name = GetValidName(Name);

                    BoolUniformNames[i] = Name;

                    Output.AppendLine($"#define {Name} (1 << {i})");
                }
            }

            Output.AppendLine();

            Output.AppendLine($"vec4 {TempRegName}[16];");
            Output.AppendLine($"bvec2 {CmpRegName};");
            Output.AppendLine($"ivec2 {A0RegName};");
            Output.AppendLine($"int {ALRegName};");

            Output.AppendLine();

            for (int i = 0; i < Program.InputRegs.Length; i++)
            {
                string Name = Program.InputRegs[i];

                if (Name != null)
                {
                    Name = GetValidName(Name);

                    InputNames[i] = Name;

                    Output.AppendLine($"layout(location = {i}) in vec4 {Name};");
                }
            }

            Output.AppendLine();

            for (int i = 0; i < Program.OutputRegs.Length; i++)
            {
                ShaderOutputReg Reg = Program.OutputRegs[i];

                if (Reg.Name == ShaderOutputRegName.TexCoord0xy)
                    Reg.Name =  ShaderOutputRegName.TexCoord0;

                if (Reg.Mask != 0)
                {
                    OutputNames[i] = $"{(HasGeoShader ? "_" : string.Empty)}{Reg.Name}";

                    Output.AppendLine($"out vec4 {OutputNames[i]};");
                }
            }

            int FuncStart = Output.Length;

            while (Procs.Count > 0)
            {
                SB = new StringBuilder(Environment.NewLine);

                GenProc(Program, Procs.Dequeue());

                Output.Insert(FuncStart, SB.ToString());
            }

            Names = GetNameBlock();

            return Output.ToString();
        }

        private void GenProc(ShaderProgram Program, ProcInfo Proc)
        {
            SB.AppendLine($"void {Proc.Name}() {{");

            Ident = "\t";

            GenProcBody(Program, Proc);

            if (string.Compare(Proc.Name, "main", true) == 0)
            {
                SB.AppendLine($"{Ident}gl_Position = {OutputNames[0]};");
            }

            SB.AppendLine("}");
        }
    }
}
