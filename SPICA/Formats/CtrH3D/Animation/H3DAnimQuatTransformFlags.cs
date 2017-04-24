using System;

namespace SPICA.Formats.CtrH3D.Animation
{
    [Flags]
    enum H3DAnimQuatTransformFlags : uint
    {
        IsTranslationConstant   = 1 << 0,
        IsRotationConstant      = 1 << 1,
        IsScaleConstant         = 1 << 2,
        IsTranslationInexistent = 1 << 3,
        IsRotationInexistent    = 1 << 4,
        IsScaleInexistent       = 1 << 5
    }
}
