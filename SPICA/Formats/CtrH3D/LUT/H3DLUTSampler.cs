using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Xml.Serialization;

namespace SPICA.Formats.CtrH3D.LUT
{
    [Inline]
    public class H3DLUTSampler : ICustomSerialization
    {
        [XmlAttribute] public H3DLUTFlags Flags;

        private byte Padding0;
        private ushort Padding1;

        private uint[] Commands;

        [XmlAttribute] public string Name;

        [Ignore, XmlAttribute] public float[] Table;

        public H3DLUTSampler()
        {
            Table = new float[256];
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            int Index = 0;

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
            uint[] QuantizedValues = new uint[256];

            for (int Index = 0; Index < Table.Length; Index++)
            {
                float Diff = 0;

                if (Index < Table.Length - 1)
                {
                    Diff = Table[Index + 1] - Table[Index];
                }

                int QVal = (int)(Table[Index] * 0xfff);
                int QDiff = (int)(Diff * 0x7ff);

                QuantizedValues[Index] = (uint)(QVal | (QDiff << 12)) & 0xffffff;
            }

            PICACommandWriter Writer = new PICACommandWriter();

            Writer.SetCommands(PICARegister.GPUREG_LIGHTING_LUT_DATA0, false, 0xf, QuantizedValues);

            Writer.WriteEnd();

            Commands = Writer.GetBuffer();

            return false;
        }
    }
}
