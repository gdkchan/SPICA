using System;

namespace SPICA.Formats.CtrGfx.Light
{
    [Flags]
    public enum GfxVertexLightFlags : uint
    {
        IsInheritingDirectionRotation = 1 << 1
    }
}
