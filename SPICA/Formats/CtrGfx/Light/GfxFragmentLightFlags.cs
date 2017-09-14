using System;

namespace SPICA.Formats.CtrGfx.Light
{
    [Flags]
    public enum GfxFragmentLightFlags : uint
    {
        IsTwoSidedDiffuse             = 1 << 1,
        IsDistanceAttenuationEnabled  = 1 << 2,
        IsInheritingDirectionRotation = 1 << 3
    }
}
