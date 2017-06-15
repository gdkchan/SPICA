namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxFragLight
    {
        public GfxFragmentFlags Flags;

        public GfxTranslucencyKind TranslucencyKind;

        public GfxFresnelSelector FresnelSelector;

        public int BumpTexture;

        public GfxBumpMode BumpMode;

        private uint BumpRenormalize;

        public bool IsBumpRenormalize
        {
            get
            {
                return BumpRenormalize != 0;
            }
            set
            {
                BumpRenormalize = value ? 1u : 0u;
            }
        }
    }
}
