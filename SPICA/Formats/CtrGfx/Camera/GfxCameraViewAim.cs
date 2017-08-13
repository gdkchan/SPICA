using System.Numerics;

namespace SPICA.Formats.CtrGfx.Camera
{
    public class GfxCameraViewAim : GfxCameraView
    {
        public GfxCameraViewAimFlags Flags;

        public Vector3 Target;

        public float Twist;
    }
}
