using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model
{
    [TypeChoice(0x02000000u, typeof(GfxSkeleton))]
    public class GfxSkeleton : GfxObject, INamed
    {
        public readonly GfxDict<GfxBone> Bones;

        public GfxBone RootBone;

        public GfxSkeletonScalingRule ScalingRule;

        private int Flags;

        public bool IsTranslationAnimEnabled
        {
            get => BitUtils.GetBit(Flags, 1);
            set => Flags = BitUtils.SetBit(Flags, value, 1);
        }
    }
}
