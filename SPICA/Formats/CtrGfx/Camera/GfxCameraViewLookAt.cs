using System.Numerics;

namespace SPICA.Formats.CtrGfx.Camera
{
    public class GfxCameraViewLookAt : GfxCameraView
    {
        public GfxCameraViewLookAtFlags Flags;

        public Vector3 Target;
        public Vector3 UpVector;
    }
}
