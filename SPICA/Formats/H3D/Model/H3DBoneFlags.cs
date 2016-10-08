using System;

namespace SPICA.Formats.H3D.Model
{
    [Flags]
    enum H3DBoneFlags : uint
    {
        IsMatrixDirty = 1 << 0,
        IsWorldMatrixUpdated = 1 << 1,
        IsCallBackEnabled = 1 << 2,
        IsSegmentScaleCompensate = 1 << 22,
        IsScaleUniform = 1 << 23,
        IsScaleVolumeOne = 1 << 24,
        IsRotationZero = 1 << 25,
        IsTranslationZero = 1 << 26,
        IsHiScaleUniform = 1 << 27,
        IsHiScaleVolumeOne = 1 << 28,
        IsHiRotationZero = 1 << 29,
        IsHiTranslationZero = 1 << 30,
    }
}
