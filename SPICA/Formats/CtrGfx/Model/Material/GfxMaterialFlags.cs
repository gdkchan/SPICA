using System;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [Flags]
    public enum GfxMaterialFlags
    {
        IsFragmentLightingEnabled    = 1 << 0,
        IsVertexLightingEnabled      = 1 << 1,
        IsHemiSphereLightingEnabled  = 1 << 2,
        IsHemiSphereOcclusionEnabled = 1 << 3,
        IsFogEnabled                 = 1 << 4,
        IsPolygonOffsetEnabled       = 1 << 5
    }
}
