using System;

namespace SPICA.Formats
{
    [Flags]
    public enum SceneContent
    {
        All     = 0x1,
        Model   = 0x2,
        Texture = 0x4,
        Light   = 0x8,
        Camera  = 0x10,
        Fog     = 0x20,
        SklAnim = 0x40,
        MatAnim = 0x80,
        VisAnim = 0x100,
        LgtAnim = 0x200,
        CamAnim = 0x400,
        FogAnim = 0x800
    }
}
