using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.LUT
{
    public class GfxLUT : INamed
    {
        private GfxVersion Revision;

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
