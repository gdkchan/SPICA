using SPICA.PICA.Commands;

namespace SPICA.Formats.Generic
{
    struct ConversionParams
    {
        public byte Compatibility;
        public ushort ConverterVersion;

        public static ConversionParams Default
        {
            get
            {
                return new ConversionParams
                {
                    Compatibility = 0x21,
                    ConverterVersion = 42607,
                };
            }
        }
    }
}
