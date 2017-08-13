using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Camera
{
    [TypeChoice(0x20000000u, typeof(GfxCameraViewRotation))]
    [TypeChoice(0x40000000u, typeof(GfxCameraViewLookAt))]
    [TypeChoice(0x80000000u, typeof(GfxCameraViewAim))]
    public class GfxCameraView { }
}
