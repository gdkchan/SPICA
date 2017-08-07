using System;

namespace SPICA.Formats.CtrGfx.Animation
{
    [Flags]
    enum GfxAnimTransformFlags
    {
        IsIdentity               = 1 << 0,
        IsRotTransZero           = 1 << 1,
        IsScaleOne               = 1 << 2,
        IsScaleUniform           = 1 << 3,
        IsRotationZero           = 1 << 4,
        IsTranslationZero        = 1 << 5,
        IsScaleXConstant         = 1 << 6,
        IsScaleYConstant         = 1 << 7,
        IsScaleZConstant         = 1 << 8,
        IsRotationXConstant      = 1 << 9,
        IsRotationYConstant      = 1 << 10,
        IsRotationZConstant      = 1 << 11,
        IsRotationWConstant      = 1 << 12,
        IsTranslationXConstant   = 1 << 13,
        IsTranslationYConstant   = 1 << 14,
        IsTranslationZConstant   = 1 << 15,
        IsScaleXInexistent       = 1 << 16,
        IsScaleYInexistent       = 1 << 17,
        IsScaleZInexistent       = 1 << 18,
        IsRotationXInexistent    = 1 << 19,
        IsRotationYInexistent    = 1 << 20,
        IsRotationZInexistent    = 1 << 21,
        IsRotationWInexistent    = 1 << 22,
        IsTranslationXInexistent = 1 << 23,
        IsTranslationYInexistent = 1 << 24,
        IsTranslationZInexistent = 1 << 25
    }
}
