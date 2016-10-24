using System;

namespace SPICA.Formats.CtrH3D.Model.Material.Texture
{
    [Flags]
    public enum H3DTextureCoordFlags : byte
    {
        IsDirty = 1 << 0
    }
}
