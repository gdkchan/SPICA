using System;

namespace SPICA.Formats.CtrH3D.Model
{
    [Flags]
    public enum H3DBoneFlags : uint
    {
        IsMatrixDirty            = 1u << 1,
        IsWorldMatrixUpdated     = 1u << 2,
        IsCallBackEnabled        = 1u << 4,
        IsSegmentScaleCompensate = 1u << 22,
        IsScaleUniform           = 1u << 23,
        IsScaleVolumeOne         = 1u << 25,
        IsRotationZero           = 1u << 26,
        IsTranslationZero        = 1u << 27,
        IsHiScaleUniform         = 1u << 28,
        IsHiScaleVolumeOne       = 1u << 29,
        IsHiRotationZero         = 1u << 30,
        IsHiTranslationZero      = 1u << 31
    }
}
