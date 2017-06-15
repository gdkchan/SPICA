using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public class GfxLUTReference
    {
        private uint Unk;

        private string _SamplerName;

        public string SamplerName
        {
            get
            {
                return _SamplerName;
            }
            set
            {
                _SamplerName = value ?? throw Exceptions.GetNullException("SamplerName");
            }
        }

        private string _TableName;

        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value ?? throw Exceptions.GetNullException("TableName");
            }
        }
    }
}
