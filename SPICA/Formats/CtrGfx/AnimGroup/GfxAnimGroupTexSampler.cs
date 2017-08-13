using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.AnimGroup
{
    class GfxAnimGroupTexSampler : GfxAnimGroupElement
    {
        private string _MaterialName;

        public string MaterialName
        {
            get => _MaterialName;
            set => _MaterialName = value ?? throw Exceptions.GetNullException("MaterialName");
        }

        public int TexSamplerIndex;

        private GfxAnimGroupObjType ObjType2;

        public GfxAnimGroupTexSampler()
        {
            ObjType = ObjType2 = GfxAnimGroupObjType.TexSampler;
        }
    }
}
