using System;

namespace SPICA.Formats.H3D.Model.Material
{
    [Flags]
    enum H3DFragLightFlags : byte
    {
        IsBumpRenormalizeEnabled = 1 << 0,
        IsClampHighLightEnabled = 1 << 1,
        IsLUTDist0Enabled = 1 << 2,
        IsLUTDist1Enabled = 1 << 3,
        IsLUTReflecEnabled = 1 << 4,
        IsLUTGeoFactor0Enabled = 1 << 5,
        IsLUTGeoFactor1Enabled = 1 << 6,
        IsLUTReflectionEnabled = 1 << 7
    }
}
