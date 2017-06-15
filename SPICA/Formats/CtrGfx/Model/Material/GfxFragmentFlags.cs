using System;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [Flags]
    public enum GfxFragmentFlags : uint
    {
        IsClampHighLightEnabled  = 1 << 0,
        IsLUTDist0Enabled        = 1 << 1,
        IsLUTDist1Enabled        = 1 << 2,
        IsLUTGeoFactor0Enabled   = 1 << 2,
        IsLUTGeoFactor1Enabled   = 1 << 3,
        IsLUTReflectionEnabled   = 1 << 4,

        //Test if any of the two Geometry Factors are enabled
        IsLUTGeoFactorEnabled =
            IsLUTGeoFactor0Enabled |
            IsLUTGeoFactor1Enabled
    }
}
