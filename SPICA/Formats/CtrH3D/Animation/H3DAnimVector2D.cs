using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimVector2D
    {
        private uint Flags;

        [Ignore] public H3DFloatKeyFrameGroup X;
        [Ignore] public H3DFloatKeyFrameGroup Y;

        public H3DAnimVector2D()
        {
            X = new H3DFloatKeyFrameGroup();
            Y = new H3DFloatKeyFrameGroup();
        }

        //TODO: Read/Write from BCH
    }
}
