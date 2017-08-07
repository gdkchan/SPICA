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

            Output.AppendLine();

            GenVec4Uniforms (Output, Program.Vec4Uniforms);
            GenIVec4Uniforms(Output, Program.IVec4Uniforms);
            GenBoolUniforms (Output, Program.BoolUniforms);

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
                    Name = $"i_{i}_{GetValidName(Name)}";

                    InputNames[i] = Name;

                    Output.AppendLine($"layout(location = {i}) in vec4 {Name};");
                }
            }

            Output.AppendLine();

            GenOutputs(Output, Program.OutputRegs, HasGeoShader ? "_" : string.Empty);

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
