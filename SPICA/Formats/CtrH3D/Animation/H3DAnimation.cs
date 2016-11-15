using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimation : INamed
    {
        public string Name;

        public H3DAnimationFlags AnimationFlags;

        public float FramesCount;

        public List<H3DAnimationElement> Elements;

        public H3DMetaData MetaData;

        public string ObjectName { get { return Name; } }

        public H3DAnimation()
        {
            Elements = new List<H3DAnimationElement>();
        }
    }
}
