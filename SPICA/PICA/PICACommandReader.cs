using System.Collections.Generic;

namespace SPICA.PICA
{
    class PICACommandReader
    {
        public int CmdIndex = 0;

        private List<PICACommand> Commands;

        public PICACommandReader(uint[] Cmds)
        {
            Commands = new List<PICACommand>();

            int Index = 0;

            while (Index < Cmds.Length)
            {
                int BaseIndex = Index;

                uint Parameter = Cmds[Index++];
                uint Command   = Cmds[Index++];

                uint Id          = (Command >>  0) & 0xffff;
                uint Mask        = (Command >> 16) & 0xf;
                uint ExtraParams = (Command >> 20) & 0x7ff;
                bool Consecutive = (Command >> 31) != 0;

                if (Consecutive)
                {
                    int Iterations = 0;

                    while (true)
                    {
                        PICACommand Cmd = new PICACommand
                        {
                            Register        = (PICARegister)Id++,
                            Parameters      = new uint[] { Parameter },
                            ParametersIndex = BaseIndex++,
                            Mask            = Mask
                        };

                        Commands.Add(Cmd);

                        if (Iterations++ == 0) BaseIndex++;

                        if (ExtraParams-- > 0)
                            Parameter = Cmds[Index++];
                        else
                            break;
                    }
                }
                else
                {
                    List<uint> Parameters = new List<uint> { Parameter };

                    for (int EP = 0; EP < ExtraParams; EP++)
                    {
                        Parameters.Add(Cmds[Index++]);
                    }

                    PICACommand Cmd = new PICACommand
                    {
                        Register        = (PICARegister)Id,
                        Parameters      = Parameters.ToArray(),
                        ParametersIndex = BaseIndex,
                        Mask            = Mask
                    };

                    Commands.Add(Cmd);
                }

                //Commands must be padded in 8 bytes blocks, so Index can't be even!
                if ((Index & 1) != 0) Index++;
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
