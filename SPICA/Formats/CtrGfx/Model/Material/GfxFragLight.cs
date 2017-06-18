namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxFragLight
    {
        public GfxFragmentFlags Flags;

        public GfxTranslucencyKind TranslucencyKind;

        public GfxFresnelSelector FresnelSelector;

        public int BumpTexture;

        public GfxBumpMode BumpMode;

        public bool IsBumpRenormalize;
    }
}
