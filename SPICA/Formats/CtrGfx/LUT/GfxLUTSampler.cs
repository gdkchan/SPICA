using SPICA.Formats.Common;
using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.LUT
{
    [TypeChoice(0x80000000u, typeof(GfxLUTSampler))]
    public class GfxLUTSampler : ICustomSerialization, INamed
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public bool IsAbsolute;

        private byte[] RawCommands;

        [Ignore] private float[] _Table;

        public float[] Table
        {
            get => _Table;
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

        public GfxLUTSampler()
        {
            _Table = new float[256];
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            uint[] Commands = new uint[RawCommands.Length >> 2];

            for (int i = 0; i < RawCommands.Length; i += 4)
            {
                Commands[i >> 2] = (uint)(
                    RawCommands[i + 0] <<  0 |
                    RawCommands[i + 1] <<  8 |
                    RawCommands[i + 2] << 16 |
                    RawCommands[i + 3] << 24);
            }

            uint Index = 0;

            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                if (Cmd.Register == PICARegister.GPUREG_LIGHTING_LUT_INDEX)
                {
                    Index = Cmd.Parameters[0] & 0xff;
                }
                else if (
                    Cmd.Register >= PICARegister.GPUREG_LIGHTING_LUT_DATA0 &&
                    Cmd.Register <= PICARegister.GPUREG_LIGHTING_LUT_DATA7)
                {
                    foreach (uint Param in Cmd.Parameters)
                    {
                        _Table[Index++] = (Param & 0xfff) / (float)0xfff;
                    }
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
