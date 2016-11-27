using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D.LUT
{
    public struct H3DLUTSampler : ICustomSerialization
    {
        public H3DLUTFlags Flags;
        private byte Padding0;
        private ushort Padding1;

        private uint[] Commands;

        public string Name;

        [Ignore] public float[] Table;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            int Index = 0;

            Table = new float[256];

            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

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

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //TODO

            return false;
        }
    }
}
