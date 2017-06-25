using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model
{
    [TypeChoice(0x02000000u, typeof(GfxSkeleton))]
    public class GfxSkeleton : INamed
    {
        private GfxRevHeader Header;

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public readonly GfxDict<GfxMetaData> MetaData;

        public readonly GfxDict<GfxBone> Bones;

        public GfxBone RootBone;

        public GfxSkeletonScalingRule ScalingRule;

        private uint Flags;

        public bool IsTranslationAnimEnabled
        {
            get => BitUtils.GetBit(Flags, 1);
            set => Flags = (uint)BitUtils.SetBit(Flags, value, 1);
        }
    }
}
