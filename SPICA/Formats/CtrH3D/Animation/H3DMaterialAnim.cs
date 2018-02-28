using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DMaterialAnim : H3DAnimation
    {
        public readonly List<string> TextureNames;

        public H3DMaterialAnim()
        {
            TextureNames = new List<string>();
        }

        public H3DMaterialAnim(H3DAnimation Anim) : this()
        {
            Name = Anim.Name;

            AnimationFlags = Anim.AnimationFlags;
            AnimationType  = Anim.AnimationType;

            CurvesCount = Anim.CurvesCount;

            FramesCount = Anim.FramesCount;

            Elements.AddRange(Anim.Elements);

            MetaData = Anim.MetaData;
        }
    }
}
