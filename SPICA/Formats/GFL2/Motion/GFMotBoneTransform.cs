using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFMotBoneTransform
    {
        public string Name;

        public bool IsAxisAngle;

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

        public GFMotBoneTransform(BinaryReader Reader, string Name, uint FramesCount) : this()
        {
            this.Name = Name;

            uint Flags = Reader.ReadUInt32();
            uint Length = Reader.ReadUInt32();

            IsAxisAngle = (Flags >> 31) == 0;

            for (int Elem = 0; Elem < 9; Elem++)
            {
                List<GFMotKeyFrame> KeyFrames = GFMotKeyFrame.ReadList(Reader, Flags, FramesCount);

                switch (Elem)
                {
                    case 0: ScaleX       = KeyFrames; break;
                    case 1: ScaleY       = KeyFrames; break;
                    case 2: ScaleZ       = KeyFrames; break;

                    case 3: RotationX    = KeyFrames; break;
                    case 4: RotationY    = KeyFrames; break;
                    case 5: RotationZ    = KeyFrames; break;

                    case 6: TranslationX = KeyFrames; break;
                    case 7: TranslationY = KeyFrames; break;
                    case 8: TranslationZ = KeyFrames; break;
                }

                Flags >>= 3;
            }
        }

        public static void SetFrameValue(List<GFMotKeyFrame> KeyFrames, float Frame, ref float Value)
        {
            if (KeyFrames.Count == 1) Value = KeyFrames[0].Value;
            if (KeyFrames.Count < 2) return;

            GFMotKeyFrame Left = KeyFrames.Last(x => x.Frame <= Frame);
            GFMotKeyFrame Right = KeyFrames.First(x => x.Frame >= Frame);

            if (Left.Frame != Right.Frame)
            {
                float FrameDiff = Frame - Left.Frame;
                float Weight = FrameDiff / (Right.Frame - Left.Frame);

                Value = Interpolation.Herp(
                    Left.Value,
                    Right.Value,
                    Left.Slope,
                    Right.Slope,
                    FrameDiff,
                    Weight);
            }
            else
            {
                Value = Left.Value;
            }
        }
    }
}
