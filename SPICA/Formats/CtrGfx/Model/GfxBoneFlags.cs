using System;

namespace SPICA.Formats.CtrGfx.Model
{
    [Flags]
    public enum GfxBoneFlags
    {
        IsIdentity               = 1 << 0,
        IsTranslationZero        = 1 << 1,
        IsRotationZero           = 1 << 2,
        IsScaleVolumeOne         = 1 << 3,
        IsScaleUniform           = 1 << 4,
        IsSegmentScaleCompensate = 1 << 5,
        IsNeededRendering        = 1 << 6,
        IsLocalMtxCalculate      = 1 << 7,
        IsWorldMtxCalculate      = 1 << 8,
        HasSkinningMtx           = 1 << 9
    }
}
