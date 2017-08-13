using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.AnimGroup
{
    class GfxAnimGroupBlendOp : GfxAnimGroupElement
    {
        private string _MaterialName;

        public string MaterialName
        {
            get => _MaterialName;
            set => _MaterialName = value ?? throw Exceptions.GetNullException("MaterialName");
        }

        private GfxAnimGroupObjType ObjType2;

        public GfxAnimGroupBlendOp()
        {
            ObjType = ObjType2 = GfxAnimGroupObjType.BlendOperation;
        }
    }
}
