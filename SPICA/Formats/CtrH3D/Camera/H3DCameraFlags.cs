using System;

namespace SPICA.Formats.CtrH3D.Camera
{
    [Flags]
    public enum H3DCameraFlags : byte
    {
        IsInheritingTargetRotation    = 1 << 0,
        IsInheritingTargetTranslation = 1 << 1,
        IsInheritingUpRotation        = 1 << 2
    }
}
