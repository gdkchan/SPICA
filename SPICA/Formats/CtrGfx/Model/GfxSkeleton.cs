using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.Model
{
    public class GfxSkeleton : INamed
    {
        private GfxVersion Header;

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

        public readonly GfxDict<GfxBone> Bones;

        public GfxBone RootBone;

        public GfxSkeletonScalingRule ScalingRule;

        private uint Flags;

        public bool IsTranslationAnimEnabled
        {
            get
            {
                return ((Flags >> 1) & 1) != 0;
            }
            set
            {
                Flags = BitUtils.SetBit(Flags, value, 1);
            }
        }
    }
}
