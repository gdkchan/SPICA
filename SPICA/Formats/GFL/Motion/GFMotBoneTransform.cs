using System;
using System.Collections.Generic;
using System.Text;

namespace SPICA.Formats.GFL.Motion
{
    class GFMotBoneTransform
    {
        public string Name;

        public List<GFMotKeyFrame> RotationX;
        public List<GFMotKeyFrame> RotationY;
        public List<GFMotKeyFrame> RotationZ;

        public List<GFMotKeyFrame> TranslationX;
        public List<GFMotKeyFrame> TranslationY;
        public List<GFMotKeyFrame> TranslationZ;

        public GFMotBoneTransform()
        {
            RotationX    = new List<GFMotKeyFrame>();
            RotationY    = new List<GFMotKeyFrame>();
            RotationZ    = new List<GFMotKeyFrame>();

            TranslationX = new List<GFMotKeyFrame>();
            TranslationY = new List<GFMotKeyFrame>();
            TranslationZ = new List<GFMotKeyFrame>();
        }
    }
}
