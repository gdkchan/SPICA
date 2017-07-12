using System.Collections.Generic;

namespace SPICA.Formats.GFL.Motion
{
    public class GF1MotBoneTransform
    {
        public string Name;

        public bool IsWorldSpace;

        public readonly List<GF1MotKeyFrame> ScaleX;
        public readonly List<GF1MotKeyFrame> ScaleY;
        public readonly List<GF1MotKeyFrame> ScaleZ;

        public readonly List<GF1MotKeyFrame> RotationX;
        public readonly List<GF1MotKeyFrame> RotationY;
        public readonly List<GF1MotKeyFrame> RotationZ;

        public readonly List<GF1MotKeyFrame> TranslationX;
        public readonly List<GF1MotKeyFrame> TranslationY;
        public readonly List<GF1MotKeyFrame> TranslationZ;

        public GF1MotBoneTransform()
        {
            ScaleX       = new List<GF1MotKeyFrame>();
            ScaleY       = new List<GF1MotKeyFrame>();
            ScaleZ       = new List<GF1MotKeyFrame>();

            RotationX    = new List<GF1MotKeyFrame>();
            RotationY    = new List<GF1MotKeyFrame>();
            RotationZ    = new List<GF1MotKeyFrame>();

            TranslationX = new List<GF1MotKeyFrame>();
            TranslationY = new List<GF1MotKeyFrame>();
            TranslationZ = new List<GF1MotKeyFrame>();
        }
    }
}
