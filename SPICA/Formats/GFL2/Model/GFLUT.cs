using SPICA.PICA;
using SPICA.PICA.Commands;
using System.IO;

namespace SPICA.Formats.GFL2.Model
{
    struct GFLUT
    {
        public PICALUTType Type;

        public float[] Table;

        public uint Hash;
        public string Name;

        public GFLUT(BinaryReader Reader, string SamplerName, int Length)
        {
            Type = default(PICALUTType);

            Hash = Reader.ReadUInt32();
            Name = SamplerName;

            Reader.BaseStream.Seek(0xc, SeekOrigin.Current);

            uint[] Commands = new uint[Length >> 2];

            for (int i = 0; i < Commands.Length; i++)
            {
                Commands[i] = Reader.ReadUInt32();
            }

            int Index = 0;

            Table = new float[256];

            

            PICACommandReader CmdReader = new PICACommandReader(Commands);

            while (CmdReader.HasCommand)
            {
                PICACommand Cmd = CmdReader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_LIGHTING_LUT_INDEX:
                        Index = (int)(Param & 0xff);
                        Type = (PICALUTType)(Param >> 8);
                        break;
                    case PICARegister.GPUREG_LIGHTING_LUT_DATA0:
                    case PICARegister.GPUREG_LIGHTING_LUT_DATA1:
                    case PICARegister.GPUREG_LIGHTING_LUT_DATA2:
                    case PICARegister.GPUREG_LIGHTING_LUT_DATA3:
                    case PICARegister.GPUREG_LIGHTING_LUT_DATA4:
                    case PICARegister.GPUREG_LIGHTING_LUT_DATA5:
                    case PICARegister.GPUREG_LIGHTING_LUT_DATA6:
                    case PICARegister.GPUREG_LIGHTING_LUT_DATA7:
                        foreach (uint Value in Cmd.Parameters)
                        {
                            Table[Index++] = (Value & 0xfff) / (float)0xfff;
                        }
                        break;
                }
            }

            System.Diagnostics.Debug.WriteLine(Type);
        }

        public void Write(BinaryWriter Writer)
        {
            //TODO
        }
    }
}
