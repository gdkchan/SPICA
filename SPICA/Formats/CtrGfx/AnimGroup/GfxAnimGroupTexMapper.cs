using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.AnimGroup
{
    class GfxAnimGroupTexMapper : GfxAnimGroupElement
    {
        private string _MaterialName;

        public string MaterialName
        {
            get => _MaterialName;
            set => _MaterialName = value ?? throw Exceptions.GetNullException("MaterialName");
        }

        public int TexMapperIndex;

        private GfxAnimGroupObjType ObjType2;

        public GfxAnimGroupTexMapper()
        {
            ObjType = ObjType2 = GfxAnimGroupObjType.TexMapper;
        }
    }
}
