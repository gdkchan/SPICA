using System;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    [Flags]
    public enum H3DFragmentFlags : byte
    {
        IsBumpRenormalizeEnabled = 1 << 0,
        IsClampHighLightEnabled  = 1 << 1,
        IsLUTDist0Enabled        = 1 << 2,
        IsLUTDist1Enabled        = 1 << 3,
        IsLUTReflectionEnabled   = 1 << 4,
        IsLUTGeoFactor0Enabled   = 1 << 5,
        IsLUTGeoFactor1Enabled   = 1 << 6,

        //Test if any of the two Geometry Factors are enabled
        IsLUTGeoFactorEnabled =
            IsLUTGeoFactor0Enabled |
            IsLUTGeoFactor1Enabled
    }
}
