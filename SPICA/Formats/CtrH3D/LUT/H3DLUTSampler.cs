using SPICA.Formats.Common;
using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D.LUT
{
    [Inline]
    public class H3DLUTSampler : ICustomSerialization, INamed
    {
        [Padding(4)] public H3DLUTFlags Flags;

        private uint[] Commands;

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value == null)
                {
                    throw Exceptions.GetNullException("Name");
                }

                _Name = value;
            }
        }

        [Ignore]
        private float[] _Table;

        public float[] Table
        {
            get
            {
                return _Table;
            }
            set
            {
                if (value == null)
                {
                    throw Exceptions.GetNullException("Table");
                }

                if (value.Length != 256)
                {
                    throw Exceptions.GetLengthNotEqualException("Table", 256);
                }

                _Table = value;
            }
        }

        public H3DLUTSampler()
        {
            _Table = new float[256];
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
                            _Table[Index++] = (Value & 0xfff) / (float)0xfff;
                        }
                        break;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            uint[] QuantizedValues = new uint[256];

            for (int Index = 0; Index < _Table.Length; Index++)
            {
                float Difference = 0;

                if (Index < _Table.Length - 1)
                {
                    Difference = _Table[Index + 1] - _Table[Index];
                }

                int Value = (int)(_Table[Index] * 0xfff);
                int Diff  = (int)(Difference    * 0x7ff);

                QuantizedValues[Index] = (uint)(Value | (Diff << 12)) & 0xffffff;
            }

            PICACommandWriter Writer = new PICACommandWriter();

            Writer.SetCommands(PICARegister.GPUREG_LIGHTING_LUT_DATA0, false, 0xf, QuantizedValues);

            Writer.WriteEnd();

            Commands = Writer.GetBuffer();

            return false;
        }
    }
}
