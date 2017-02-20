using System;

namespace SPICA.Formats.CtrH3D.Model
{
    [Flags]
    public enum H3DModelFlags : byte
    {
        IsDrawingEnabled  = 1 << 0,
        HasSkeleton       = 1 << 1,
        HasSubMeshCulling = 1 << 2,
        HasSilhouette     = 1 << 3
    }
}
