using System.Numerics;

namespace SPICA.Formats.CtrGfx.Camera
{
    public class GfxCameraViewLookAt : GfxCameraView
    {
        public GfxCameraViewLookAtFlags Flags;

        public Vector3 TargetPosition;
        public Vector3 UpVector;
    }
}
