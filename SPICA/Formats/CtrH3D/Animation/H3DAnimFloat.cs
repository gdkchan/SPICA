using System;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimFloat
    {
        private uint Flags;

        [NonSerialized] public H3DFloatKeyFrameGroup Value;

        public H3DAnimFloat()
        {
            Value = new H3DFloatKeyFrameGroup();
        }

        //TODO: Read/Write from BCH
    }
}
