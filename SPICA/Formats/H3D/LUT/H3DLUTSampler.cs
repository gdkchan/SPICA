using SPICA.PICA;
using SPICA.Serialization;

using System;

namespace SPICA.Formats.H3D.LUT
{
    class H3DLUTSampler : ICustomDeserializer
    {
        public H3DLUTFlags Flags;
        public byte Padding0;
        public ushort Padding1;

        public uint[] Commands;

        public string Name;

        [NonSerialized]
        public float[] Table;

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                Table = new float[256];

                int Index = 0;

                switch (Cmd.Register)
                {
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
        }
    }
}
