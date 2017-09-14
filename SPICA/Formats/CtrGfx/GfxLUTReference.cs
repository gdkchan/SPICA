using SPICA.Formats.Common;
using SPICA.Formats.CtrGfx.LUT;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx
{
    [TypeChoice(0x40000000u, typeof(GfxLUTReference))]
    public class GfxLUTReference
    {
        private string _SamplerName;

        public string SamplerName
        {
            get => _SamplerName;
            set => _SamplerName = value ?? throw Exceptions.GetNullException("SamplerName");
        }

        private string _TableName;

        public string TableName
        {
            get => _TableName;
            set => _TableName = value ?? throw Exceptions.GetNullException("TableName");
        }

        public GfxLUT TargetLUT;
    }
}
