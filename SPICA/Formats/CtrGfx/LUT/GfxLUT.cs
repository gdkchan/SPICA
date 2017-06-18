using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.LUT
{
    [TypeChoice(0x04000000u, typeof(GfxLUT))]
    public class GfxLUT : INamed
    {
        private GfxRevHeader Header;

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

        public readonly GfxDict<GfxMetaData> MetaData;

        public readonly GfxDict<GfxLUTSampler> Samplers;
    }
}
