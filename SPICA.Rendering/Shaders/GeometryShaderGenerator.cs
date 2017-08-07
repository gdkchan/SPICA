using SPICA.PICA.Shader;

using System;
using System.Text;

namespace SPICA.Rendering.Shaders
{
    class GeometryShaderGenerator : ShaderGenerator
    {
        private uint EmitFlags;

        public GeometryShaderGenerator(ShaderBinary SHBin) : base(SHBin) { }

        public string GetGeoShader(int ProgramIndex, string[] VtxShaderOutputs, out ShaderNameBlock Names)
        {
            ShaderProgram Program = SHBin.Programs[ProgramIndex];

            Initialize(Program);

            StringBuilder Output = new StringBuilder();

            Output.AppendLine("//SPICA auto-generated code");
            Output.AppendLine("//This code was translated from a MAESTRO Geometry Shader");
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
            Output.AppendLine("int geo_i;");

            Output.AppendLine();

            Output.AppendLine("layout(triangles) in;");
            Output.AppendLine("layout(triangle_strip, max_vertices = 12) out;");

            Output.AppendLine();

            for (int i = 0; i < VtxShaderOutputs.Length; i++)
            {
                if (VtxShaderOutputs[i] != null)
                {
                    InputNames[i] = $"{VtxShaderOutputs[i]}[geo_i]";

                    Output.AppendLine($"in vec4 {VtxShaderOutputs[i]}[];");
                }
            }

            Output.AppendLine();

            GenOutputs(Output, Program.OutputRegs);

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
            if (string.Compare(Proc.Name, "main", true) != 0)
            {
                SB.AppendLine($"void {Proc.Name}() {{");

                Ident = "\t";

                GenProcBody(Program, Proc);

                SB.AppendLine("}");
            }
            else
            {
                //TODO: Support other geometry shader types.
                //Currently, only Point Sprites are supported,
                //but some models uses silhouette, and maybe subdivision too.
                SB.AppendLine($"void main() {{");
                SB.AppendLine("\tfor (geo_i = 0; geo_i < 3; geo_i++) {");

                Ident = "\t\t";

                GenProcBody(Program, Proc);

                SB.AppendLine("\t\tEndPrimitive();");
                SB.AppendLine("\t}");
                SB.AppendLine("}");
            }
        }

        protected override void SetEmit(ShaderProgram Program, uint InstOp)
        {
            EmitFlags = InstOp;
        }

        protected override void GenEmit(ShaderProgram Program, uint InstOp)
        {
            SB.AppendLine($"{Ident}gl_Position = {ShaderOutputRegName.Position};");
            SB.AppendLine($"{Ident}EmitVertex();");
        }
    }
}
