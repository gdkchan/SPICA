using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Camera
{
    [TypeChoice(0x20000000u, typeof(GfxCameraProjectionPerspective))]
    [TypeChoice(0x40000000u, typeof(GfxCameraProjectionOrthogonal))]
    [TypeChoice(0x80000000u, typeof(GfxCameraProjectionFrustum))]
    public class GfxCameraProjection
    {
        public float ZNear;
        public float ZFar;
    }
}
