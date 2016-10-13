using System;

namespace SPICA.Formats.H3D
{
    [Flags]
    enum H3DFlags : byte
    {
        IsFromNewConverter = 1 << 0,
        IsInitialized = 1 << 1,
        IsUnInitDisabled = 1 << 2
    }
}
