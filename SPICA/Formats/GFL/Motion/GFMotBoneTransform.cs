using System.Collections.Generic;

namespace SPICA.Formats.GFL.Motion
{
    class GFMotBoneTransform
    {
        public string Name;

        public readonly List<GFMotKeyFrame> ScaleX;
        public readonly List<GFMotKeyFrame> ScaleY;
        public readonly List<GFMotKeyFrame> ScaleZ;

        public readonly List<GFMotKeyFrame> RotationX;
        public readonly List<GFMotKeyFrame> RotationY;
        public readonly List<GFMotKeyFrame> RotationZ;

        public readonly List<GFMotKeyFrame> TranslationX;
        public readonly List<GFMotKeyFrame> TranslationY;
        public readonly List<GFMotKeyFrame> TranslationZ;

        public GFMotBoneTransform()
        {
            ScaleX       = new List<GFMotKeyFrame>();
            ScaleY       = new List<GFMotKeyFrame>();
            ScaleZ       = new List<GFMotKeyFrame>();

            RotationX    = new List<GFMotKeyFrame>();
            RotationY    = new List<GFMotKeyFrame>();
            RotationZ    = new List<GFMotKeyFrame>();

            TranslationX = new List<GFMotKeyFrame>();
            TranslationY = new List<GFMotKeyFrame>();
            TranslationZ = new List<GFMotKeyFrame>();
        }
    }
}
