using SPICA.Utils;
using System.Collections.Generic;

namespace SPICA.PICA
{
    class PICACommandWriter
    {
        private List<uint> Commands;

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

        public void SetCommand(PICARegister Register, float Param)
        {
            Commands.Add(IOUtils.ToUInt(Param));
            Commands.Add((uint)Register | (0xf << 16));
        }

        public void SetCommand(PICARegister Register, bool Param)
        {
            Commands.Add(Param ? 1u : 0u);
            Commands.Add((uint)Register | (0xf << 16));
        }

        public void SetCommand(PICARegister Register, bool Consecutive = false, uint Mask = 0xf, params uint[] Params)
        {
            Commands.Add(Params[0]);

            uint ExtraW = (uint)(((Params.Length - 1) & 0x7ff) << 20);
            uint CFlag = Consecutive ? (1u << 31) : 0;

            Commands.Add((uint)Register | (Mask << 16) | ExtraW | CFlag);

            for (int PIndex = 1; PIndex < Params.Length; PIndex++)
            {
                Commands.Add(Params[PIndex]);
            }

            Align();
        }

        public void SetCommand(PICARegister Register, bool Consecutive = false, params float[] Params)
        {
            Commands.Add(IOUtils.ToUInt(Params[0]));

            uint ExtraW = (uint)(((Params.Length - 1) & 0x7ff) << 20);
            uint CFlag = Consecutive ? (1u << 31) : 0;

            Commands.Add((uint)Register | (0xf << 16) | ExtraW | CFlag);

            for (int PIndex = 1; PIndex < Params.Length; PIndex++)
            {
                Commands.Add(IOUtils.ToUInt(Params[PIndex]));
            }

            Align();
        }

        private void Align()
        {
            if ((Commands.Count & 1) != 0) Commands.Add(0);
        }
    }
}
