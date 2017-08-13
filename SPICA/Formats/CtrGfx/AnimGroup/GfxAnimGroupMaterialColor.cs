using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.AnimGroup
{
    class GfxAnimGroupMaterialColor : GfxAnimGroupElement
    {
        private string _MaterialName;

        public string MaterialName
        {
            get => _MaterialName;
            set => _MaterialName = value ?? throw Exceptions.GetNullException("MaterialName");
        }

        private GfxAnimGroupObjType ObjType2;

        public GfxAnimGroupMaterialColor()
        {
            ObjType = ObjType2 = GfxAnimGroupObjType.MaterialColor;
        }
    }
}
