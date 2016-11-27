using SPICA.Math3D;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DFloatKeyFrameGroup : ICustomSerialization
    {
        public float StartFrame;
        public float EndFrame;

        public H3DLoopType PreRepeat;
        public H3DLoopType PostRepeat;

        public ushort CurveIndex;

        public H3DInterpolationType InterpolationType;
        private H3DSegmentQuantization SegmentQuantization;

        private ushort Count;

        private float ValueScale;
        private float ValueOffset;
        private float FrameScale;
        private float InvDuration;

        [Ignore] public List<H3DFloatKeyFrame> KeyFrames;

        public bool HasData { get { return KeyFrames.Count > 0; } }

        public H3DFloatKeyFrameGroup()
        {
            KeyFrames = new List<H3DFloatKeyFrame>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Deserializer.BaseStream.Seek(Deserializer.Reader.ReadUInt32(), SeekOrigin.Begin);

            for (int Index = 0; Index < Count; Index++)
            {
                H3DFloatKeyFrame KeyFrame = new H3DFloatKeyFrame();

                switch (SegmentQuantization)
                {
                    //Hermite
                    case  H3DSegmentQuantization.Hermite128:
                        KeyFrame.Frame = Deserializer.Reader.ReadSingle();
                        KeyFrame.Value = Deserializer.Reader.ReadSingle();
                        KeyFrame.InSlope = Deserializer.Reader.ReadSingle();
                        KeyFrame.OutSlope = Deserializer.Reader.ReadSingle();
                        break;

                    case H3DSegmentQuantization.Hermite64:
                        uint H64Value = Deserializer.Reader.ReadUInt32();

                        KeyFrame.Frame = H64Value & 0xfff;
                        KeyFrame.Value = H64Value >> 12;
                        KeyFrame.InSlope = Deserializer.Reader.ReadInt16() / 256f;
                        KeyFrame.OutSlope = Deserializer.Reader.ReadInt16() / 256f;
                        break;

                    case H3DSegmentQuantization.Hermite48:
                        KeyFrame.Frame = Deserializer.Reader.ReadByte();
                        KeyFrame.Value = Deserializer.Reader.ReadUInt16();

                        int Slope0 = Deserializer.Reader.ReadByte();
                        int Slope1 = Deserializer.Reader.ReadByte();
                        int Slope2 = Deserializer.Reader.ReadByte();

                        KeyFrame.InSlope = (((Slope0 | ((Slope1 & 0xf) << 8)) << 20) >> 20) / 32f;
                        KeyFrame.OutSlope = ((((Slope1 >> 4) | (Slope2 << 4)) << 20) >> 20) / 32f;
                        break;

                    //Hermite (unified Slope)
                    case H3DSegmentQuantization.UnifiedHermite96:
                        KeyFrame.Frame = Deserializer.Reader.ReadSingle();
                        KeyFrame.Value = Deserializer.Reader.ReadSingle();
                        KeyFrame.InSlope = Deserializer.Reader.ReadSingle();
                        KeyFrame.OutSlope = KeyFrame.InSlope;
                        break;

                    case H3DSegmentQuantization.UnifiedHermite48:
                        KeyFrame.Frame = Deserializer.Reader.ReadUInt16() / 32f;
                        KeyFrame.Value = Deserializer.Reader.ReadUInt16();
                        KeyFrame.InSlope = Deserializer.Reader.ReadInt16() / 256f;
                        KeyFrame.OutSlope = KeyFrame.InSlope;
                        break;

                    case H3DSegmentQuantization.UnifiedHermite32:
                        KeyFrame.Frame = Deserializer.Reader.ReadByte();

                        ushort UH32Value = Deserializer.Reader.ReadUInt16();
                        byte HighSlope = Deserializer.Reader.ReadByte();

                        KeyFrame.Value = UH32Value & 0xfff;
                        KeyFrame.InSlope = ((((UH32Value >> 12) | (HighSlope << 4)) << 20) >> 20) / 32f;
                        KeyFrame.OutSlope = KeyFrame.InSlope;
                        break;

                    //Step/Linear
                    case H3DSegmentQuantization.StepLinear64:
                        KeyFrame.Frame = Deserializer.Reader.ReadSingle();
                        KeyFrame.Value = Deserializer.Reader.ReadSingle();
                        break;

                    case H3DSegmentQuantization.StepLinear32:
                        uint SL32Value = Deserializer.Reader.ReadUInt32();

                        KeyFrame.Frame = SL32Value & 0xfff;
                        KeyFrame.Value = SL32Value >> 12;
                        break;
                }

                KeyFrame.Frame = (KeyFrame.Frame * FrameScale);
                KeyFrame.Value = (KeyFrame.Value * ValueScale) + ValueOffset;

                KeyFrames.Add(KeyFrame);
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            throw new NotImplementedException();
        }

        internal static H3DFloatKeyFrameGroup ReadGroup(BinaryDeserializer Deserializer, bool Constant)
        {
            H3DFloatKeyFrameGroup FrameGrp = new H3DFloatKeyFrameGroup();

            if (Constant)
            {
                FrameGrp.KeyFrames.Add(new H3DFloatKeyFrame
                {
                    Frame = 0,
                    Value = Deserializer.Reader.ReadSingle()
                });
            }
            else
            {
                uint Address = Deserializer.Reader.ReadUInt32();

                Deserializer.BaseStream.Seek(Address, SeekOrigin.Begin);

                FrameGrp = Deserializer.Deserialize<H3DFloatKeyFrameGroup>();
            }

            return FrameGrp;
        }

        public float GetFrameValue(float Frame)
        {
            if (KeyFrames.Count == 0) return 0;
            if (KeyFrames.Count == 1) return KeyFrames[0].Value;

            H3DFloatKeyFrame Left = KeyFrames.Last(x => x.Frame <= Frame);
            H3DFloatKeyFrame Right = KeyFrames.First(x => x.Frame >= Frame);

            if (Left.Frame != Right.Frame)
            {
                float FrameDiff = Frame - Left.Frame;
                float Weight = FrameDiff / (Right.Frame - Left.Frame);

                switch (InterpolationType)
                {
                    case H3DInterpolationType.Step: return Left.Value;
                    case H3DInterpolationType.Linear: return Interpolation.Lerp(Left.Value, Right.Value, Weight);
                    case H3DInterpolationType.Hermite:
                        return Interpolation.Herp(
                            Left.Value,
                            Right.Value,
                            Left.OutSlope,
                            Right.InSlope,
                            FrameDiff,
                            Weight);

                    default: throw new ArgumentException("Invalid Interpolation type!");
                }
            }
            else
            {
                return Left.Value;
            }
        }
    }
}
