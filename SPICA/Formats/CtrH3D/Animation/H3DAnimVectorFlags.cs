using System;

namespace SPICA.Formats.CtrH3D.Animation
{
    [Flags]
    enum H3DAnimVectorFlags : uint
    {
        IsXConstant   = 1 << 0,
        IsYConstant   = 1 << 1,
        IsZConstant   = 1 << 2,
        IsWConstant   = 1 << 3,
        IsXInexistent = 1 << 8,
        IsYInexistent = 1 << 9,
        IsZInexistent = 1 << 10,
        IsWInexistent = 1 << 11,
        IsXActive     = 1 << 16,
        IsYActive     = 1 << 17,
        IsZActive     = 1 << 18,
        IsWActive     = 1 << 19
    }
}
