using System;

namespace SPICA.Formats.CtrH3D.Light
{
    [Flags]
    public enum H3DLightFlags : byte
    {
        IsTwoSidedDiffuse      = 1 << 0,
        HasDistanceAttenuation = 1 << 1
    }
}
