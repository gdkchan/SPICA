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

        public GFMotBoneTransform(BinaryReader Reader, string Name, uint FramesCount) : this()
        {
            this.Name = Name;

            uint Flags  = Reader.ReadUInt32();
            uint Length = Reader.ReadUInt32();

            IsAxisAngle = (Flags >> 31) == 0;

            for (int ElemIndex = 0; ElemIndex < 9; ElemIndex++)
            {
                switch (ElemIndex)
                {
                    case 0: GFMotKeyFrame.SetList(ScaleX,       Reader, Flags, FramesCount); break;
                    case 1: GFMotKeyFrame.SetList(ScaleY,       Reader, Flags, FramesCount); break;
                    case 2: GFMotKeyFrame.SetList(ScaleZ,       Reader, Flags, FramesCount); break;

                    case 3: GFMotKeyFrame.SetList(RotationX,    Reader, Flags, FramesCount); break;
                    case 4: GFMotKeyFrame.SetList(RotationY,    Reader, Flags, FramesCount); break;
                    case 5: GFMotKeyFrame.SetList(RotationZ,    Reader, Flags, FramesCount); break;

                    case 6: GFMotKeyFrame.SetList(TranslationX, Reader, Flags, FramesCount); break;
                    case 7: GFMotKeyFrame.SetList(TranslationY, Reader, Flags, FramesCount); break;
                    case 8: GFMotKeyFrame.SetList(TranslationZ, Reader, Flags, FramesCount); break;
                }

                Flags >>= 3;
            }
        }

        public static void SetFrameValue(List<GFMotKeyFrame> KeyFrames, float Frame, ref float Value)
        {
            if (KeyFrames.Count == 1) Value = KeyFrames[0].Value;
            if (KeyFrames.Count < 2) return;

            GFMotKeyFrame LHS = KeyFrames.Last(x => x.Frame <= Frame);
            GFMotKeyFrame RHS = KeyFrames.First(x => x.Frame >= Frame);

            if (LHS.Frame != RHS.Frame)
            {
                float FrameDiff = Frame - LHS.Frame;
                float Weight = FrameDiff / (RHS.Frame - LHS.Frame);

                Value = Interpolation.Herp(
                    LHS.Value, RHS.Value,
                    LHS.Slope, RHS.Slope,
                    FrameDiff,
                    Weight);
            }
            else
            {
                Value = LHS.Value;
            }
        }
    }
}
