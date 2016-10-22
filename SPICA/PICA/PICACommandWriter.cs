using SPICA.Utils;
using System.Collections.Generic;

namespace SPICA.PICA
{
    class PICACommandWriter
    {
        private List<uint> Commands;

        public int Index { get { return Commands.Count; } }

        public PICACommandWriter()
        {
            Commands = new List<uint>();
        }

        public uint[] GetBuffer()
        {
            return Commands.ToArray();
        }

        public void SetCommand(PICARegister Register, uint Param, uint Mask = 0xf)
        {
            Commands.Add(Param);
            Commands.Add((uint)Register | (Mask << 16));
        }

        public void SetCommand(PICARegister Register, float Param, uint Mask = 0xf)
        {
            Commands.Add(IOUtils.ToUInt32(Param));
            Commands.Add((uint)Register | (Mask << 16));
        }

        public void SetCommand(PICARegister Register, bool Param, uint Mask = 0xf)
        {
            Commands.Add(Param ? 1u : 0u);
            Commands.Add((uint)Register | (Mask << 16));
        }

        public void SetCommand(PICARegister Register, params bool[] Params)
        {
            uint Param = 0;

            for (int Bit = 0; Bit < Params.Length; Bit++)
            {
                if (Params[Bit]) Param |= (1u << Bit);
            }

            SetCommand(Register, Param, 1);
        }

        public void SetCommand(PICARegister Register, bool Consecutive, params uint[] Params)
        {
            Commands.Add(Params[0]);

            uint WordsCount = (((uint)Params.Length - 1) & 0x7ff) << 20;
            uint ConsecutiveBit = Consecutive ? (1u << 31) : 0;

            Commands.Add((uint)Register | (0xf << 16) | WordsCount | ConsecutiveBit);

            for (int Index = 1; Index < Params.Length; Index++)
            {
                Commands.Add(Params[Index]);
            }

            Align();
        }

        public void SetCommand(PICARegister Register, bool Consecutive, params float[] Params)
        {
            Commands.Add(IOUtils.ToUInt32(Params[0]));

            uint WordsCount = (((uint)Params.Length - 1) & 0x7ff) << 20;
            uint ConsecutiveBit = Consecutive ? (1u << 31) : 0;

            Commands.Add((uint)Register | (0xf << 16) | WordsCount | ConsecutiveBit);

            for (int Index = 1; Index < Params.Length; Index++)
            {
                Commands.Add(IOUtils.ToUInt32(Params[Index]));
            }

            Align();
        }

        public void WriteEnd()
        {
            //Make sure that the Buffer is aligned on a 16 bytes boundary
            if ((Index & 3) == 0)
            {
                SetCommand(PICARegister.GPUREG_DUMMY, 0, 0);
            }

            SetCommand(PICARegister.GPUREG_CMDBUF_JUMP1, true);
        }

        private void Align()
        {
            if ((Commands.Count & 1) != 0) Commands.Add(0);
        }
    }
}
