using SPICA.Formats.Common;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.Numerics;

namespace SPICA.PICA
{
    class PICACommandReader
    {
        public int CmdIndex = 0;

        private List<PICACommand> Commands;

        public readonly Vector4[] Uniforms;

        private PICAVectorFloat24 VectorF24;

        private uint UniformIndex;
        private bool Uniform32Bits;

        public PICACommandReader(uint[] Cmds)
        {
            Commands = new List<PICACommand>();

            Uniforms = new Vector4[96];

            int Index = 0;

            while (Index < Cmds.Length)
            {
                uint Parameter = Cmds[Index++];
                uint Command   = Cmds[Index++];

                uint Id          = (Command >>  0) & 0xffff;
                uint Mask        = (Command >> 16) & 0xf;
                uint ExtraParams = (Command >> 20) & 0x7ff;
                bool Consecutive = (Command >> 31) != 0;

                if (Consecutive)
                {
                    for (int i = 0; i < ExtraParams + 1; i++)
                    {
                        PICACommand Cmd = new PICACommand()
                        {
                            Register   = (PICARegister)Id++,
                            Parameters = new uint[] { Parameter },
                            Mask       = Mask
                        };

                        CheckUniformsCmd(Cmd);

                        Commands.Add(Cmd);

                        if (i < ExtraParams)
                        {
                            Parameter = Cmds[Index++];
                        }
                    }
                }
                else
                {
                    List<uint> Parameters = new List<uint> { Parameter };

                    for (int i = 0; i < ExtraParams; i++)
                    {
                        Parameters.Add(Cmds[Index++]);
                    }

                    PICACommand Cmd = new PICACommand()
                    {
                        Register   = (PICARegister)Id,
                        Parameters = Parameters.ToArray(),
                        Mask       = Mask
                    };

                    CheckUniformsCmd(Cmd);

                    Commands.Add(Cmd);
                }

                //Commands must be padded in 8 bytes blocks, so Index can't be even!
                if ((Index & 1) != 0) Index++;
            }
        }

        private void CheckUniformsCmd(PICACommand Cmd)
        {
            //Check if command is a Uniform, and update Uniform registers if needed
            if (Cmd.Register == PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX)
            {
                UniformIndex  = (Cmd.Parameters[0] & 0xff) << 2;
                Uniform32Bits = (Cmd.Parameters[0] >> 31) != 0;
            }
            else if (
                Cmd.Register >= PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA0 &&
                Cmd.Register <= PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA7)
            {
                foreach (uint Param in Cmd.Parameters)
                {
                    uint UIdx = UniformIndex >> 2;

                    if (Uniform32Bits)
                    {
                        float Value = IOUtils.ToSingle(Param);

                        switch (UniformIndex & 3)
                        {
                            case 0: Uniforms[UIdx].W = Value; break;
                            case 1: Uniforms[UIdx].Z = Value; break;
                            case 2: Uniforms[UIdx].Y = Value; break;
                            case 3: Uniforms[UIdx].X = Value; break;
                        }
                    }
                    else
                    {
                        switch (UniformIndex & 3)
                        {
                            case 0: VectorF24.Word0 = Param; break;
                            case 1: VectorF24.Word1 = Param; break;
                            case 2: VectorF24.Word2 = Param; break;
                        }

                        if ((UniformIndex & 3) == 2)
                        {
                            //The Float 24 Vector only uses 3 Words (24 * 4 = 96 bits = 3 Words)
                            //for all four elements (X/Y/Z/W), so we ignore the fourth Word here
                            UniformIndex++;

                            Uniforms[UIdx] = VectorF24;
                        }
                    }

                    UniformIndex++;
                }
            }
        }

        public bool HasCommand
        {
            get { return CmdIndex < Commands.Count; }
        }

        public PICACommand GetCommand()
        {
            return Commands[CmdIndex++];
        }

        public PICACommand[] GetCommands()
        {
            return Commands.ToArray();
        }
    }
}
