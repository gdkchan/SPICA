using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.GFL2.Motion
{
    class GFMotBoneTransform
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
            InitLists();
        }

        public GFMotBoneTransform(BinaryReader Reader, string Name)
        {
            InitLists();

            this.Name = Name;

            uint Flags = Reader.ReadUInt32();
            uint Length = Reader.ReadUInt32();

            IsAxisAngle = (Flags >> 31) == 0;

            for (int Elem = 0; Elem < 9; Elem++)
            {
                List<GFMotKeyFrame> KeyFrames = new List<GFMotKeyFrame>();

                switch (Flags & 7)
                {
                    case 3: KeyFrames.Add(new GFMotKeyFrame(0, Reader.ReadSingle(), 0)); break; //Constant

                    case 4: //Quantized Key Frame list
                        uint KeyFramesCount = Reader.ReadUInt32();

                        byte[] Frames = new byte[KeyFramesCount];

                        for (int Index = 0; Index < KeyFramesCount; Index++)
                        {
                            Frames[Index] = Reader.ReadByte();
                        }

                        while ((Reader.BaseStream.Position & 3) != 0) Reader.ReadByte();

                        float ValueScale = Reader.ReadSingle();
                        float ValueOffset = Reader.ReadSingle();
                        float SlopeScale = Reader.ReadSingle();
                        float SlopeOffset = Reader.ReadSingle();

                        for (int Index = 0; Index < KeyFramesCount; Index++)
                        {
                            KeyFrames.Add(new GFMotKeyFrame
                            {
                                Frame = Frames[Index],
                                Value = (Reader.ReadUInt16() / (float)ushort.MaxValue) * ValueScale + ValueOffset,
                                Slope = (Reader.ReadUInt16() / (float)ushort.MaxValue) * SlopeScale + SlopeOffset
                            });
                        }
                        break;
                }

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

        private void InitLists()
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

        public static void SetFrameValue(List<GFMotKeyFrame> KeyFrames, float Frame, ref float Value)
        {
            if (KeyFrames.Count == 1) Value = KeyFrames[0].Value;
            if (KeyFrames.Count < 2) return;

            GFMotKeyFrame Left = KeyFrames.Last(x => x.Frame <= Frame);
            GFMotKeyFrame Right = KeyFrames.First(x => x.Frame >= Frame);

            if (Left.Frame != Right.Frame)
            {
                float F = Frame - Left.Frame;
                float W = F / (Right.Frame - Left.Frame);

                float IS = Right.Slope;
                float OS = Left.Slope;

                float L = Left.Value;
                float R = Right.Value;

                float W1 = W - 1;

                Value = L + (L - R) * (2 * W - 3) * W * W;
                Value += (F * W1) * (IS * W + OS * W1);
            }
            else
            {
                Value = Left.Value;
            }
        }
    }
}
