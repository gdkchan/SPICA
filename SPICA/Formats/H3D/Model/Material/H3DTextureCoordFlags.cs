using System;

namespace SPICA.Formats.H3D.Model.Material
{
    [Flags]
    enum H3DTextureCoordFlags : byte
    {
        IsDirty = 1 << 0
    }
}
