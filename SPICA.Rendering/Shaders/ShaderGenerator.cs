using SPICA.PICA.Shader;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SPICA.Rendering.Shaders
{
    class ShaderGenerator
    {
        public const string BoolsName = "BoolUniforms";

        protected const string TempRegName = "reg_temp";
        protected const string A0RegName   = "reg_a0";
        protected const string ALRegName   = "reg_al";
        protected const string CmpRegName  = "reg_cmp";

        protected string[] Vec4UniformNames;
        protected string[] Vec4UniformNamesNoIdx;
        protected string[] IVec4UniformNames;
        protected string[] BoolUniformNames;
        protected string[] InputNames;
        protected string[] OutputNames;

        private delegate void GenInstCode(ShaderProgram Program, uint InstOp);

        private GenInstCode[] InstTbl;

        protected struct ProcInfo
        {
            public string Name;
            public uint   Offset;
            public uint   Length;
        }

        protected Queue<ProcInfo> Procs;

        protected HashSet<string> ProcTbl;

        protected Dictionary<uint, string> Labels;

        protected ShaderBinary SHBin;

        protected StringBuilder SB;
        
        protected string Ident;

        private uint IP;

        public ShaderGenerator(ShaderBinary SHBin)
        {
            this.SHBin = SHBin;

            Vec4UniformNames      = new string[96];
            Vec4UniformNamesNoIdx = new string[96];
            IVec4UniformNames     = new string[4];
            BoolUniformNames      = new string[16];
            InputNames            = new string[16];
            OutputNames           = new string[16];

            InstTbl = new GenInstCode[]
            {
                new GenInstCode(GenInst1), //Add
                new GenInstCode(GenInst1), //DP3
                new GenInstCode(GenInst1), //DP4
                new GenInstCode(GenInst1), //DPH
                new GenInstCode(GenInst1), //Dst
                new GenInstCode(GenInst1), //Ex2
                new GenInstCode(GenInst1), //Lg2
                new GenInstCode(GenInst1u), //LitP
                new GenInstCode(GenInst1), //Mul
                new GenInstCode(GenInst1), //SGE
                new GenInstCode(GenInst1), //SLT
                new GenInstCode(GenInst1u), //Flr
                new GenInstCode(GenInst1), //Max
                new GenInstCode(GenInst1), //Min
                new GenInstCode(GenInst1u), //Rcp
                new GenInstCode(GenInst1u), //RSq
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenInst1u), //MovA
                new GenInstCode(GenInst1u), //Mov
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenInst1i), //DPHI
                new GenInstCode(GenInst1i), //DstI
                new GenInstCode(GenInst1i), //SGEI
                new GenInstCode(GenInst1i), //SLTI
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //Invalid?
                new GenInstCode(GenNOp), //NOp
                new GenInstCode(GenEnd), //End
                new GenInstCode(GenInst2), //BreakC
                new GenInstCode(GenInst2), //Call
                new GenInstCode(GenInst2), //CallC
                new GenInstCode(GenInst3), //CallU
                new GenInstCode(GenInst3), //IfU
                new GenInstCode(GenInst2), //IfC
                new GenInstCode(GenInst3), //Loop
                new GenInstCode(GenEmit), //Emit
                new GenInstCode(SetEmit), //SetEmit
                new GenInstCode(GenInst2), //JmpC
                new GenInstCode(GenInst3), //JmpU
                new GenInstCode(GenCmp), //Cmp
                new GenInstCode(GenCmp), //Cmp
                new GenInstCode(GenMAdI), //MAdI
                new GenInstCode(GenMAdI), //MAdI
                new GenInstCode(GenMAdI), //MAdI
                new GenInstCode(GenMAdI), //MAdI
                new GenInstCode(GenMAdI), //MAdI
                new GenInstCode(GenMAdI), //MAdI
                new GenInstCode(GenMAdI), //MAdI
                new GenInstCode(GenMAdI), //MAdI
                new GenInstCode(GenMAd), //MAd
                new GenInstCode(GenMAd), //MAd
                new GenInstCode(GenMAd), //MAd
                new GenInstCode(GenMAd), //MAd
                new GenInstCode(GenMAd), //MAd
                new GenInstCode(GenMAd), //MAd
                new GenInstCode(GenMAd), //MAd
                new GenInstCode(GenMAd) //MAd
            };
        }

        /* Initialization */

        protected void Initialize(ShaderProgram Program)
        {
            Procs = new Queue<ProcInfo>();

            ProcTbl = new HashSet<string>();

            Labels = new Dictionary<uint, string>();

            Procs.Enqueue(new ProcInfo()
            {
                Name   = "main",
                Offset = Program.MainOffset,
                Length = Program.EndMainOffset - Program.MainOffset
            });

            foreach (ShaderLabel Lbl in Program.Labels)
            {
                if (Lbl.Name != "endmain")
                {
                    Labels.Add(Lbl.Offset, Lbl.Name);
                }
            }
        }

        protected void GenVec4Uniforms(StringBuilder Output, ShaderUniformVec4[] Uniforms)
        {
            GenVec4Uniforms(Output, Uniforms, Vec4UniformNames, "vec4");
        }

        protected void GenIVec4Uniforms(StringBuilder Output, ShaderUniformVec4[] Uniforms)
        {
            GenVec4Uniforms(Output, Uniforms, IVec4UniformNames, "ivec4");
        }

        private void GenVec4Uniforms(StringBuilder Output, ShaderUniformVec4[] Uniforms, string[] Names, string Type)
        {
            for (int i = 0; i < Uniforms.Length; i++)
            {
                ShaderUniformVec4 Uniform = Uniforms[i];

                if (Uniform?.IsConstant ?? true) continue;

                string Name = $"{Type}_{i - Uniform.ArrayIndex}_{GetValidName(Uniform.Name)}";

                /*
                 * For registers used as arrays, the name is stored with the
                 * indexer ([0], [1], [2]...), but a version without the indexer
                 * is also stored in Vec4UniformNamesNoIdx for GetSrcReg func, since it
                 * needs indexed array access with illegal memory access protection.
                 */
                string Indexer = Uniform.IsArray ? $"[{Uniform.ArrayIndex}]" : string.Empty;

                Names[i] = Name + Indexer;

                if (Names == Vec4UniformNames)
                {
                    Vec4UniformNamesNoIdx[i] = Name;
                }

                if (Uniform.ArrayIndex == 0)
                {
                    if (Uniform.IsArray)
                        Output.AppendLine($"uniform {Type} {Name}[{Uniform.ArrayLength}];");
                    else
                        Output.AppendLine($"uniform {Type} {Name};");
                }
            }
        }

        protected void GenBoolUniforms(StringBuilder Output, ShaderUniformBool[] Uniforms)
        {
            Output.AppendLine($"uniform int {BoolsName};");

            Output.AppendLine();

            for (int i = 0; i < Uniforms.Length; i++)
            {
                string Name = $"bool_{i}_{GetValidName(Uniforms[i].Name)}";

                BoolUniformNames[i] = Name;

                Output.AppendLine($"#define {Name} (1 << {i})");
            }
        }

        protected void GenOutputs(StringBuilder Output, ShaderOutputReg[] Regs, string Prefix = "")
        {
            for (int i = 0; i < Regs.Length; i++)
            {
                ShaderOutputReg Reg = Regs[i];

                if (Reg.Mask != 0)
                {
                    if (Reg.Name == ShaderOutputRegName.TexCoord0W)
                        Reg.Name =  ShaderOutputRegName.TexCoord0;

                    OutputNames[i] = $"{Prefix}{Reg.Name}";

                    //Shaders can have more than one generic output.
                    //In this case we need to add a suffix to avoid name clashes.
                    if (Reg.Name == ShaderOutputRegName.Generic)
                    {
                        OutputNames[i] += "_" + i;
                    }

                    Output.AppendLine($"out vec4 {OutputNames[i]};");
                }
            }
        }

        protected void GenProcBody(ShaderProgram Program, ProcInfo Proc)
        {
            for (IP = Proc.Offset; IP < Proc.Offset + Proc.Length; IP++)
            {
                //Split procedure if a label is found at current address.
                //This is done to support Jump instructions.
                if (IP > Proc.Offset && Labels.ContainsKey(IP))
                {
                    string Name = Labels[IP];

                    AddProc(Name, IP, (Proc.Offset + Proc.Length) - IP);

                    SB.AppendLine($"\t{Name}();");

                    break;
                }

                //Generate current instruction.
                GenInst(Program, SHBin.Executable[IP]);
            }
        }

        /* Helper methods for derived classes */
        
        protected ShaderNameBlock GetNameBlock()
        {
            return new ShaderNameBlock(
                Vec4UniformNames,
                IVec4UniformNames,
                BoolUniformNames,
                InputNames,
                OutputNames);
        }

        protected string GetValidName(string Name)
        {
            StringBuilder Output = new StringBuilder();

            foreach (char c in Name)
            {
                if ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9') ||
                    (c == '_'))
                {
                    Output.Append(c);
                }
            }

            return Output.ToString();
        }

        /* Instructions CodeGen */

        private void GenInst(ShaderProgram Program, uint Inst)
        {
            InstTbl[Inst >> 26](Program, Inst);
        }

        private void GenInst1(ShaderProgram Program, uint InstOp)
        {
            ShaderOpCode OpCode = (ShaderOpCode)(InstOp >> 26);

            ShaderInst1 Inst = new ShaderInst1(InstOp);

            string Dest = GetDstReg(Program, Inst.Dest);
            string Src1 = GetSrcReg(Program, Inst.Src1, Inst.Idx1);
            string Src2 = GetSrcReg(Program, Inst.Src2);

            SHBin.GetSwizzles(
                Inst.Desc,
                out string SDst,
                out string[] SSrc,
                out string[] SSrcM);

            string[] Signs = SHBin.GetSrcSigns(Inst.Desc);

            Src1 = Signs[0] + Src1;
            Src2 = Signs[1] + Src2;

            switch (OpCode)
            {
                case ShaderOpCode.Add:
                    Append($"{Dest}.{SDst} = {Src1}.{SSrcM[0]} + {Src2}.{SSrcM[1]};");
                    break;

                case ShaderOpCode.DP3:
                    GenDP3(Src1, Src2, SSrc[0], SSrc[1], Dest, SDst);
                    break;

                case ShaderOpCode.DP4:
                    Append($"{Dest}.{SDst} = {GetVecCtor($"dot({Src1}.{SSrc[0]}, {Src2}.{SSrc[1]})", SDst.Length)};");
                    break;

                case ShaderOpCode.DPH:
                    GenDPH(Src1, Src2, SSrc[0], SSrc[1], Dest, SDst);
                    break;

                case ShaderOpCode.Dst:
                    Append($"{Dest} = vec4(1, {Src1}.y * {Src1}.y, {Src1}.z, {Src2}.w);");
                    break;

                case ShaderOpCode.Mul:
                    Append($"{Dest}.{SDst} = {Src1}.{SSrcM[0]} * {Src2}.{SSrcM[1]};");
                    break;

                case ShaderOpCode.SGE:
                    GenSGE(Src1, Src1, SSrcM[0], SSrcM[1], Dest, SDst);
                    break;

                case ShaderOpCode.SLT:
                    GenSLT(Src1, Src1, SSrcM[0], SSrcM[1], Dest, SDst);
                    break;

                case ShaderOpCode.Max:
                    Append($"{Dest}.{SDst} = max({Src1}.{SSrcM[0]}, {Src2}.{SSrcM[1]});");
                    break;

                case ShaderOpCode.Min:
                    Append($"{Dest}.{SDst} = min({Src1}.{SSrcM[0]}, {Src2}.{SSrcM[1]});");
                    break;
            }
        }

        private void GenInst1i(ShaderProgram Program, uint InstOp)
        {
            ShaderOpCode OpCode = (ShaderOpCode)(InstOp >> 26);

            ShaderInst1i Inst = new ShaderInst1i(InstOp);

            string Dest = GetDstReg(Program, Inst.Dest);
            string Src1 = GetSrcReg(Program, Inst.Src1);
            string Src2 = GetSrcReg(Program, Inst.Src2, Inst.Idx2);

            SHBin.GetSwizzles(
                Inst.Desc,
                out string SDst,
                out string[] SSrc,
                out string[] SSrcM);

            string[] Signs = SHBin.GetSrcSigns(Inst.Desc);

            Src1 = Signs[0] + Src1;
            Src2 = Signs[1] + Src2;

            switch (OpCode)
            {
                case ShaderOpCode.DPHI:
                    GenDPH(Src1, Src2, SSrc[0], SSrc[1], Dest, SDst);
                    break;

                case ShaderOpCode.DstI:
                    Append($"{Dest} = vec4(1, {Src1}.y * {Src1}.y, {Src1}.z, {Src2}.w);");
                    break;

                case ShaderOpCode.SGEI:
                    GenSGE(Src1, Src1, SSrcM[0], SSrcM[1], Dest, SDst);
                    break;

                case ShaderOpCode.SLTI:
                    GenSLT(Src1, Src1, SSrcM[0], SSrcM[1], Dest, SDst);
                    break;
            }
        }

        private void GenDP3(
            string Src1,  string Src2,
            string SSrc1, string SSrc2,
            string Dest,  string SDst)
        {
            string Dot = GetVecCtor($"dot(" +
                $"{Src1}.{SSrc1.Substring(0, 3)}, " +
                $"{Src2}.{SSrc2.Substring(0, 3)})", SDst.Length);

            Append($"{Dest}.{SDst} = {Dot};");
        }

        private void GenDPH(
            string Src1,  string Src2,
            string SSrc1, string SSrc2,
            string Dest,  string SDst)
        {
            string Dot = GetVecCtor($"dot(" +
                $"{Src1}.{SSrc1.Substring(0, 3)}, " +
                $"{Src2}.{SSrc2.Substring(0, 3)}) + {Src2}.w", SDst.Length);

            Append($"{Dest}.{SDst} = {Dot};");
        }

        private void GenSGE(
            string Src1,  string Src2,
            string SSrc1, string SSrc2,
            string Dest,  string SDst)
        {
            for (int i = 0; i < SDst.Length; i++)
            {
                Append(
                    $"{Dest}.{SDst[i]} = " +
                    $"{Src1}.{SSrc1[i]} >= " +
                    $"{Src2}.{SSrc2[i]} ? 1 : 0;");
            }
        }

        private void GenSLT(
            string Src1,  string Src2,
            string SSrc1, string SSrc2,
            string Dest,  string SDst)
        {
            for (int i = 0; i < SDst.Length; i++)
            {
                Append(
                    $"{Dest}.{SDst[i]} = " +
                    $"{Src1}.{SSrc1[i]} < " +
                    $"{Src2}.{SSrc2[i]} ? 1 : 0;");
            }
        }

        private void GenInst1u(ShaderProgram Program, uint InstOp)
        {
            ShaderOpCode OpCode = (ShaderOpCode)(InstOp >> 26);

            ShaderInst1 Inst = new ShaderInst1(InstOp);

            string Dest = GetDstReg(Program, Inst.Dest);
            string Src1 = GetSrcReg(Program, Inst.Src1, Inst.Idx1);

            SHBin.GetSwizzles(
                Inst.Desc,
                out string SDst,
                out string[] SSrc,
                out string[] SSrcM);

            string[] Signs = SHBin.GetSrcSigns(Inst.Desc);

            Src1 = Signs[0] + Src1;

            switch (OpCode)
            {
                case ShaderOpCode.Ex2:
                    Append($"{Dest}.{SDst} = exp2({Src1}.{SSrcM[0]});");
                    break;

                case ShaderOpCode.Lg2:
                    Append($"{Dest}.{SDst} = log2({Src1}.{SSrcM[0]});");
                    break;

                case ShaderOpCode.LitP:
                    GenLitP(Src1, Dest);
                    break;

                case ShaderOpCode.Flr:
                    Append($"{Dest}.{SDst} = floor({Src1}.{SSrcM[0]});");
                    break;

                case ShaderOpCode.Rcp:
                    Append($"{Dest}.{SDst} = {GetVecCtor("1", SDst.Length)} / {Src1}.{SSrcM[0]};");
                    break;

                case ShaderOpCode.RSq:
                    Append($"{Dest}.{SDst} = inversesqrt({Src1}.{SSrcM[0]});");
                    break;

                case ShaderOpCode.MovA:
                    GenMovA(Src1, SSrcM[0], SDst);
                    break;

                case ShaderOpCode.Mov:
                    Append($"{Dest}.{SDst} = {Src1}.{SSrcM[0]};");
                    break;
            }
        }

        private void GenLitP(string Src, string Dest)
        {
            Append($"{Dest} = vec4(max({Src}.x, 0), clamp({Src}.y, -1, 1), 0, max({Src}.w, 0));");
            Append($"{CmpRegName}.x = {Src}.x > 0 ? 1 : 0;");
            Append($"{CmpRegName}.y = {Src}.w > 0 ? 1 : 0;");
        }

        private void GenMovA(string Src, string SSrc, string SDst)
        {
            int Length = SDst.Length;

            if (Length > 2)
                Length = 2;

            SDst = SDst.Substring(0, Length);
            SSrc = SSrc.Substring(0, Length);

            Append(Length > 1
                ? $"{A0RegName}.{SDst} = ivec2({Src}.{SSrc});"
                : $"{A0RegName}.{SDst} = int({Src}.{SSrc});");
        }

        private void GenInst2(ShaderProgram Program, uint InstOp)
        {
            ShaderOpCode OpCode = (ShaderOpCode)(InstOp >> 26);

            ShaderInst2 Inst = new ShaderInst2(InstOp);

            switch (OpCode)
            {
                case ShaderOpCode.BreakC: GenBreakC(Program, Inst); break;
                case ShaderOpCode.Call:   GenCall  (Program, Inst); break;
                case ShaderOpCode.CallC:  GenCallC (Program, Inst); break;
                case ShaderOpCode.IfC:    GenIfC   (Program, Inst); break;
                case ShaderOpCode.JmpC:   GenJmpC  (Program, Inst); break;
            }
        }

        private void GenInst3(ShaderProgram Program, uint InstOp)
        {
            ShaderOpCode OpCode = (ShaderOpCode)(InstOp >> 26);

            ShaderInst3 Inst = new ShaderInst3(InstOp);

            switch (OpCode)
            {
                case ShaderOpCode.IfU:   GenIfU  (Program, Inst); break;
                case ShaderOpCode.CallU: GenCallU(Program, Inst); break;
                case ShaderOpCode.Loop:  GenLoop (Program, Inst); break;
                case ShaderOpCode.JmpU:  GenJmpU (Program, Inst); break;
            }
        }

        private void GenBreakC(ShaderProgram Program, ShaderInst2 Inst)
        {
            Append($"if ({GetCond(Inst)}) break;");
        }

        private void GenCall(ShaderProgram Program, ShaderInst2 Inst)
        {
            Append(GetCall(Program, Inst.Dest, Inst.Count));
        }

        private void GenCallC(ShaderProgram Program, ShaderInst2 Inst)
        {
            Append($"if ({GetCond(Inst)}) {GetCall(Program, Inst.Dest, Inst.Count)}");
        }

        private void GenCallU(ShaderProgram Program, ShaderInst3 Inst)
        {
            Append($"if ({GetBoolCond(Program, Inst.RegId)}) {GetCall(Program, Inst.Dest, Inst.Count)}");
        }

        private string GetCall(ShaderProgram Program, uint Dest, uint Count)
        {
            string Name = Labels[Dest];

            AddProc(Name, Dest, Count);

           return $"{Name}();";
        }

        private void GenIfC(ShaderProgram Program, ShaderInst2 Inst)
        {
            GenIf(Program, GetCond(Inst), Inst.Dest, Inst.Count);
        }

        private void GenIfU(ShaderProgram Program, ShaderInst3 Inst)
        {
            GenIf(Program, GetBoolCond(Program, Inst.RegId), Inst.Dest, Inst.Count);
        }

        private void GenIf(ShaderProgram Program, string Cond, uint Dest, uint Count)
        {
            Append($"if ({Cond}) {{");

            string OldIdent = Ident;

            Ident += "\t";

            while (IP + 1 < Dest)
            {
                GenInst(Program, SHBin.Executable[++IP]);
            }

            if (Count > 0)
            {
                SB.AppendLine($"{OldIdent}}} else {{");

                while (IP + 1 < Dest + Count)
                {
                    GenInst(Program, SHBin.Executable[++IP]);
                }
            }

            Ident = OldIdent;

            Append("}");
        }

        private void GenJmpC(ShaderProgram Program, ShaderInst2 Inst)
        {
            GenJmp(Program, GetCond(Inst), Inst.Dest);
        }

        private void GenJmpU(ShaderProgram Program, ShaderInst3 Inst)
        {
            GenJmp(Program, GetBoolCond(Program, Inst.RegId, (Inst.Count & 1) == 0), Inst.Dest);
        }

        private void GenJmp(ShaderProgram Program, string Cond, uint Dest)
        {
            Append($"if ({Cond}) {{ //Jump");
            Append($"\t{Labels[Dest]}();");
            Append("\treturn;");
            Append("}");
        }

        private void GenLoop(ShaderProgram Program, ShaderInst3 Inst)
        {
            ShaderUniformVec4 Uniform = Program.IVec4Uniforms[Inst.RegId & 3];

            string IUName = IVec4UniformNames[Inst.RegId & 3];

            string ALStart;
            string ALCond;
            string ALInc;

            if (Uniform.IsConstant)
            {
                ALStart = $"{ALRegName} = {(byte)Uniform.Constant.Y}";
                ALCond  = $"{ALRegName} <= {(byte)Uniform.Constant.X}";
                ALInc   = $"{ALRegName} += {(byte)Uniform.Constant.Z}";  
            }
            else
            {
                ALStart = $"{ALRegName} = {IUName}.y";
                ALCond  = $"{ALRegName} <= {IUName}.x";
                ALInc   = $"{ALRegName} += {IUName}.z";  
            }

            Append($"for ({ALStart}; {ALCond}; {ALInc}) {{");

            string OldIdent = Ident;

            Ident += "\t";

            while (IP + 1 <= Inst.Dest)
            {
                GenInst(Program, SHBin.Executable[++IP]);
            }

            Ident = OldIdent;

            Append("}");
        }

        protected virtual void SetEmit(ShaderProgram Program, uint InstOp) { }

        protected virtual void GenEmit(ShaderProgram Program, uint InstOp) { }

        private void GenCmp(ShaderProgram Program, uint InstOp)
        {
            ShaderInst1c Inst = new ShaderInst1c(InstOp);

            SHBin.GetSwizzles(
                Inst.Desc,
                out string SDst,
                out string[] SSrc,
                out string[] SSrcM);

            string[] Signs = SHBin.GetSrcSigns(Inst.Desc);

            string Src1 = Signs[0] + GetSrcReg(Program, Inst.Src1, Inst.Idx1);
            string Src2 = Signs[1] + GetSrcReg(Program, Inst.Src2);

            string CmpX = GetComparison($"{Src1}.{SSrc[0][0]}", $"{Src2}.{SSrc[1][0]}", Inst.CmpX);
            string CmpY = GetComparison($"{Src1}.{SSrc[0][1]}", $"{Src2}.{SSrc[1][1]}", Inst.CmpY);

            Append($"{CmpRegName}.x = {CmpX};");
            Append($"{CmpRegName}.y = {CmpY};");
        }

        private void GenMAd(ShaderProgram Program, uint InstOp)
        {
            ShaderInstMAd Inst = new ShaderInstMAd(InstOp);

            string Dest = GetDstReg(Program, Inst.Dest);
            string Src1 = GetSrcReg(Program, Inst.Src1);
            string Src2 = GetSrcReg(Program, Inst.Src2, Inst.Idx2);
            string Src3 = GetSrcReg(Program, Inst.Src3);

            SHBin.GetSwizzles(
                Inst.Desc,
                out string SDst,
                out string[] SSrc,
                out string[] SSrcM);

            string[] Signs = SHBin.GetSrcSigns(Inst.Desc);

            Append($"{Dest}.{SDst} = " +
                $"{Signs[0]}{Src1}.{SSrcM[0]} * " +
                $"{Signs[1]}{Src2}.{SSrcM[1]} + " +
                $"{Signs[2]}{Src3}.{SSrcM[2]};");
        }

        private void GenMAdI(ShaderProgram Program, uint InstOp)
        {
            ShaderInstMAdI Inst = new ShaderInstMAdI(InstOp);

            string Dest = GetDstReg(Program, Inst.Dest);
            string Src1 = GetSrcReg(Program, Inst.Src1);
            string Src2 = GetSrcReg(Program, Inst.Src2);
            string Src3 = GetSrcReg(Program, Inst.Src3, Inst.Idx3);

            SHBin.GetSwizzles(
                Inst.Desc,
                out string SDst,
                out string[] SSrc,
                out string[] SSrcM);

            string[] Signs = SHBin.GetSrcSigns(Inst.Desc);

            Append($"{Dest}.{SDst} = " +
                $"{Signs[0]}{Src1}.{SSrcM[0]} * " +
                $"{Signs[1]}{Src2}.{SSrcM[1]} + " +
                $"{Signs[2]}{Src3}.{SSrcM[2]};");
        }

        private void GenNOp(ShaderProgram Program, uint InstOp) { }

        private void GenEnd(ShaderProgram Program, uint InstOp) { }

        /* Static helper methods */

        private static string GetVecCtor(string Result, int Length)
        {
            switch (Length)
            {
                case 2: return $"vec2({Result})";
                case 3: return $"vec3({Result})";
                case 4: return $"vec4({Result})";
            }

            return Result;
        }

        private static string GetVecCast(string Result, int Length)
        {
            switch (Length)
            {
                case 1: return $"float({Result})";
                case 2: return $"vec2({Result})";
                case 3: return $"vec3({Result})";
                case 4: return $"vec4({Result})";
            }

            return Result;
        }

        private static string GetCond(ShaderInst2 Inst)
        {
            string RefX = Inst.RefX ? string.Empty : "!";
            string RefY = Inst.RefY ? string.Empty : "!";

            string Cond = string.Empty;

            switch (Inst.CondOp)
            {
                case 0: Cond = $"{RefX}{CmpRegName}.x || {RefY}{CmpRegName}.y"; break;
                case 1: Cond = $"{RefX}{CmpRegName}.x && {RefY}{CmpRegName}.y"; break;
                case 2: Cond = $"{RefX}{CmpRegName}.x";                         break;
                case 3: Cond = $"{RefY}{CmpRegName}.y";                         break;
            }

            return Cond;
        }

        private static string GetComparison(string LHS, string RHS, uint Cmp)
        {
            switch (Cmp)
            {
                case 0: return $"{LHS} == {RHS}";
                case 1: return $"{LHS} != {RHS}";
                case 2: return $"{LHS} < {RHS}";
                case 3: return $"{LHS} <= {RHS}";
                case 4: return $"{LHS} > {RHS}";
                case 5: return $"{LHS} >= {RHS}";
            }

            return "true";
        }

        /* Instance helper methods */

        private void Append(string Code)
        {
            SB.AppendLine(Ident + Code);
        }

        private string GetSrcReg(ShaderProgram Program, uint Reg, uint Idx = 0)
        {
            if (Reg >= 0x20 && Reg < 0x80)
            {
                ShaderUniformVec4 Uniform = Program.Vec4Uniforms[Reg - 0x20];

                string Name = Vec4UniformNames[Reg - 0x20];

                if (Uniform.IsConstant)
                {
                    return string.Format(CultureInfo.InvariantCulture, "vec4({0}, {1}, {2}, {3})",
                        Uniform.Constant.X,
                        Uniform.Constant.Y,
                        Uniform.Constant.Z,
                        Uniform.Constant.W);
                }
                else if (Uniform.IsArray && Idx > 0)
                {
                    //Min protects against illegal accesses (can cause glitches on some GPUs).
                    Name = Vec4UniformNamesNoIdx[Reg - 0x20];

                    int Max = Uniform.ArrayLength - 1;

                    switch (Idx)
                    {
                        case 1: return $"{Name}[min({Uniform.ArrayIndex} + {A0RegName}.x, {Max})]";
                        case 2: return $"{Name}[min({Uniform.ArrayIndex} + {A0RegName}.y, {Max})]";
                        case 3: return $"{Name}[min({Uniform.ArrayIndex} + {ALRegName}, {Max})]";
                    }
                }

                return Name;
            }
            else if (Reg < 0x10)
            {
                return InputNames[Reg];
            }
            else if (Reg < 0x20)
            {
                return $"{TempRegName}[{Reg - 0x10}]";
            }
            else
            {
                throw new InvalidOperationException($"Invalid register Index {Reg} used!");
            }
        }

        private string GetDstReg(ShaderProgram Program, uint Reg)
        {
            if (Reg < 0x10)
            {
                return OutputNames[Reg];
            }
            else if (Reg < 0x20)
            {
                return $"{TempRegName}[{Reg - 0x10}]";
            }
            else
            {
                throw new InvalidOperationException($"Invalid register Index {Reg} used!");
            }
        }

        private string GetBoolCond(ShaderProgram Program, uint RegId, bool OnTrue = true)
        {
            if (Program.BoolUniforms[RegId].IsConstant)
            {
                return Program.BoolUniforms[RegId].Constant ? "true" : "false";
            }
            else if (OnTrue)
            {
                return $"({BoolsName} & {BoolUniformNames[RegId]}) != 0";
            }
            else
            {
                return $"({BoolsName} & {BoolUniformNames[RegId]}) == 0";
            }
        }

        private void AddProc(string Name, uint Offset, uint Length)
        {
            if (!ProcTbl.Contains(Name))
            {
                ProcTbl.Add(Name);

                Procs.Enqueue(new ProcInfo()
                {
                    Name   = Name,
                    Offset = Offset,
                    Length = Length
                });
            }
        }
    }
}
