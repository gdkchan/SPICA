using System.Collections.Generic;

namespace SPICA.Formats.GFL.Motion
{
    class GFMotBoneTransform
    {
        public string Name;

        public List<GFMotKeyFrame> ScaleX;
        public List<GFMotKeyFrame> ScaleY;
        public List<GFMotKeyFrame> ScaleZ;

        public List<GFMotKeyFrame> RotationX;
        public List<GFMotKeyFrame> RotationY;
        public List<GFMotKeyFrame> RotationZ;

        public List<GFMotKeyFrame> TranslationX;
        public List<GFMotKeyFrame> TranslationY;
        public List<GFMotKeyFrame> TranslationZ;

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
