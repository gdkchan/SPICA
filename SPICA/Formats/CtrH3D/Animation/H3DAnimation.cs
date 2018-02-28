using SPICA.Formats.Common;

using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimation : INamed
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public H3DAnimationFlags AnimationFlags;
        public H3DAnimationType  AnimationType;

        public ushort CurvesCount;

        public float FramesCount;

        public readonly List<H3DAnimationElement> Elements;

        public H3DMetaData MetaData;

        public H3DAnimation()
        {
            Elements = new List<H3DAnimationElement>();
        }
    }
}
