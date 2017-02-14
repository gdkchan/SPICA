using System;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    [Flags]
    public enum H3DMaterialFlags : ushort
    {
        IsFragmentLightingEnabled             = 1 << 0,
        IsVertexLightingEnabled               = 1 << 1,
        IsHemiSphereLightingEnabled           = 1 << 2,
        IsHemiSphereOcclusionEnabled          = 1 << 3,
        IsFogEnabled                          = 1 << 4,
        IsPolygonOffsetEnabled                = 1 << 5,
        IsParamCommandSourceAccessible        = 1 << 6,
        IsFragmentLightingCommandCacheUpdated = 1 << 7,
        IsFragmentLightingPolygonOffsetDirty  = 1 << 8,
        IsLightColorMutable                   = 1 << 9,
        IsCombinerConstantMutable             = 1 << 10,
        IsBlendColorMutable                   = 1 << 11,
        IsTextureCoord0Mutable                = 1 << 12,
        IsTextureCoord1Mutable                = 1 << 13,
        IsTextureCoord2Mutable                = 1 << 14,
        IsTextureBorderColorMutable           = 1 << 15
    }
}
