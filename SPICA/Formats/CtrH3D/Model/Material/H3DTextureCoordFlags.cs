using System;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    [Flags]
    public enum H3DTextureCoordFlags : byte
    {
        IsDirty = 1 << 0
    }
}
