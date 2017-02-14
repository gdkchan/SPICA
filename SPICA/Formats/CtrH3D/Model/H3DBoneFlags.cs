using System;

namespace SPICA.Formats.CtrH3D.Model
{
    [Flags]
    public enum H3DBoneFlags : uint
    {
        IsMatrixDirty            = 1 << 1,
        IsWorldMatrixUpdated     = 1 << 2,
        IsCallBackEnabled        = 1 << 4,
        IsSegmentScaleCompensate = 1 << 22,
        IsScaleUniform           = 1 << 23,
        IsScaleVolumeOne         = 1 << 24,
        IsRotationZero           = 1 << 25,
        IsTranslationZero        = 1 << 26,
        IsHiScaleUniform         = 1 << 27,
        IsHiScaleVolumeOne       = 1 << 28,
        IsHiRotationZero         = 1 << 29,
        IsHiTranslationZero      = 1 << 30,
    }
}
