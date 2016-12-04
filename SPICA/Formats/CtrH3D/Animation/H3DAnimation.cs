using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimation : INamed
    {
        private string _Name;

        public H3DAnimationFlags AnimationFlags;

        public float FramesCount;

        public List<H3DAnimationElement> Elements;

        public H3DMetaData MetaData;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public H3DAnimation()
        {
            Elements = new List<H3DAnimationElement>();
        }
    }
}
