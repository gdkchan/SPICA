using SPICA.Formats.Common;
using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.LUT
{
    public class GfxLUTSampler : ICustomSerialization, INamed
    {
        private uint Unk;

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value ?? throw Exceptions.GetNullException("Name");
            }
        }

        private uint Absolute;

        public bool IsAbsolute
        {
            get
            {
                return Absolute != 0;
            }
            set
            {
                Absolute = value ? 1u : 0u;
            }
        }

        private uint CommandsByteLength;

        [FixedLength(258)] private uint[] Commands;

        [Ignore] private float[] _Table;

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

        public GfxLUTSampler()
        {
            _Table = new float[256];
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
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
