using SPICA.Formats.Common;
using SPICA.Math3D;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DFloatKeyFrameGroup : H3DAnimationCurve, ICustomSerialization
    {
        [IfVersion(CmpOp.Gequal, 0x20)] public H3DInterpolationType   InterpolationType;
        [IfVersion(CmpOp.Gequal, 0x20)] public H3DSegmentQuantization SegmentQuantization;

        [IfVersion(CmpOp.Gequal, 0x20)] private ushort Count;

        [IfVersion(CmpOp.Gequal, 0x20)] private float ValueScale;
        [IfVersion(CmpOp.Gequal, 0x20)] private float ValueOffset;
        [IfVersion(CmpOp.Gequal, 0x20)] private float FrameScale;
        [IfVersion(CmpOp.Gequal, 0x20)] private float InvDuration;

        [Ignore] public readonly List<KeyFrame> KeyFrames;

        public bool Exists => KeyFrames.Count > 0;

        public H3DFloatKeyFrameGroup()
        {
            KeyFrames = new List<KeyFrame>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            if (Deserializer.FileVersion < 0x20)
            {
                /*
                 * Older version have a pointer within the curve data,
                 * instead of storing it directly. So we read it below...
                 */
                Deserializer.BaseStream.Seek(Deserializer.Reader.ReadUInt32(), SeekOrigin.Begin);

                //We don't need this since it's already stored on Curve
                float StartFrame = Deserializer.Reader.ReadSingle();
                float EndFrame   = Deserializer.Reader.ReadSingle();

                InterpolationType   = (H3DInterpolationType)Deserializer.Reader.ReadByte();
                SegmentQuantization = (H3DSegmentQuantization)Deserializer.Reader.ReadByte();

                Count = Deserializer.Reader.ReadUInt16();

                InvDuration = Deserializer.Reader.ReadSingle();
                ValueScale  = Deserializer.Reader.ReadSingle();
                ValueOffset = Deserializer.Reader.ReadSingle();
                FrameScale  = Deserializer.Reader.ReadSingle();
            }

            Deserializer.BaseStream.Seek(Deserializer.Reader.ReadUInt32(), SeekOrigin.Begin);

            for (int Index = 0; Index < Count; Index++)
            {
                KeyFrame KF;

                switch (SegmentQuantization)
                {
                    case H3DSegmentQuantization.Hermite128:       KF = Deserializer.Reader.ReadHermite128();       break;
                    case H3DSegmentQuantization.Hermite64:        KF = Deserializer.Reader.ReadHermite64();        break;
                    case H3DSegmentQuantization.Hermite48:        KF = Deserializer.Reader.ReadHermite48();        break;
                    case H3DSegmentQuantization.UnifiedHermite96: KF = Deserializer.Reader.ReadUnifiedHermite96(); break;
                    case H3DSegmentQuantization.UnifiedHermite48: KF = Deserializer.Reader.ReadUnifiedHermite48(); break;
                    case H3DSegmentQuantization.UnifiedHermite32: KF = Deserializer.Reader.ReadUnifiedHermite32(); break;
                    case H3DSegmentQuantization.StepLinear64:     KF = Deserializer.Reader.ReadStepLinear64();     break;
                    case H3DSegmentQuantization.StepLinear32:     KF = Deserializer.Reader.ReadStepLinear32();     break;

                    default: throw new InvalidOperationException($"Invalid Segment quantization {SegmentQuantization}!");
                }

                float OldVal = KF.Value;

                KF.Frame = KF.Frame * FrameScale;
                KF.Value = KF.Value * ValueScale + ValueOffset;

                KeyFrames.Add(KF);
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            if (Exists)
            {
                Count = (ushort)KeyFrames.Count;

                float MinFrame = KeyFrames[0].Frame;
                float MaxFrame = KeyFrames[0].Frame;
                float MinValue = KeyFrames[0].Value;
                float MaxValue = KeyFrames[0].Value;

                for (int Index = 1; Index < Count; Index++)
                {
                    KeyFrame KF = KeyFrames[Index];

                    if (KF.Frame < MinFrame) MinFrame = KF.Frame;
                    if (KF.Frame > MaxFrame) MaxFrame = KF.Frame;
                    if (KF.Value < MinValue) MinValue = KF.Value;
                    if (KF.Value > MaxValue) MaxValue = KF.Value;
                }

                ValueScale  = MaxValue - MinValue;
                ValueOffset = MinValue;
                FrameScale  = MaxFrame - MinFrame;
                InvDuration = 1f / EndFrame;

                /*
                 * Frame and Value quantization scales based on Segment quantization.
                 * Different encoding modes have different amount of bits available for each field.
                 */
                const float q8  = 1f / 0xff;
                const float q12 = 1f / 0xfff;
                const float q16 = 1f / 0xffff;
                const float q20 = 1f / 0xfffff;

                switch (SegmentQuantization)
                {
                    case H3DSegmentQuantization.Hermite128:       FrameScale  = 1f;  ValueScale  = 1f;  break;
                    case H3DSegmentQuantization.Hermite64:        FrameScale *= q12; ValueScale *= q20; break;
                    case H3DSegmentQuantization.Hermite48:        FrameScale *= q8;  ValueScale *= q16; break;
                    case H3DSegmentQuantization.UnifiedHermite96: FrameScale  = 1f;  ValueScale  = 1f;  break;
                    case H3DSegmentQuantization.UnifiedHermite48: FrameScale  = 1f;  ValueScale *= q16; break;
                    case H3DSegmentQuantization.UnifiedHermite32: FrameScale *= q8;  ValueScale *= q12; break;
                    case H3DSegmentQuantization.StepLinear64:     FrameScale  = 1f;  ValueScale  = 1f;  break;
                    case H3DSegmentQuantization.StepLinear32:     FrameScale *= q12; ValueScale *= q20; break;

                    default: throw new InvalidOperationException($"Invalid Segment quantization {SegmentQuantization}!");
                }

                if (ValueScale == 1)
                {
                    /*
                     * Quantizations were the value scale is not needed (like the ones that already stores the value
                     * as float) will ignore the offset aswell, so we need to set to to zero.
                     */
                    ValueOffset = 0;
                }

                Serializer.WriteValue(this);

                if (Serializer.FileVersion < 0x20)
                {
                    Serializer.WritePointer((uint)Serializer.BaseStream.Position + 4);

                    Serializer.Writer.Write(StartFrame);
                    Serializer.Writer.Write(EndFrame);

                    Serializer.Writer.Write((byte)InterpolationType);
                    Serializer.Writer.Write((byte)SegmentQuantization);

                    Serializer.Writer.Write(Count);

                    Serializer.Writer.Write(InvDuration);
                    Serializer.Writer.Write(ValueScale);
                    Serializer.Writer.Write(ValueOffset);
                    Serializer.Writer.Write(FrameScale);
                }

                foreach (KeyFrame Key in KeyFrames)
                {
                    KeyFrame KF = Key;

                    KF.Frame = (KF.Frame / FrameScale);
                    KF.Value = (KF.Value - ValueOffset) / ValueScale;                   

                    switch (SegmentQuantization)
                    {
                        case H3DSegmentQuantization.Hermite128:       Serializer.Writer.WriteHermite128(KF);       break;
                        case H3DSegmentQuantization.Hermite64:        Serializer.Writer.WriteHermite64(KF);        break;
                        case H3DSegmentQuantization.Hermite48:        Serializer.Writer.WriteHermite48(KF);        break;
                        case H3DSegmentQuantization.UnifiedHermite96: Serializer.Writer.WriteUnifiedHermite96(KF); break;
                        case H3DSegmentQuantization.UnifiedHermite48: Serializer.Writer.WriteUnifiedHermite48(KF); break;
                        case H3DSegmentQuantization.UnifiedHermite32: Serializer.Writer.WriteUnifiedHermite32(KF); break;
                        case H3DSegmentQuantization.StepLinear64:     Serializer.Writer.WriteStepLinear64(KF);     break;
                        case H3DSegmentQuantization.StepLinear32:     Serializer.Writer.WriteStepLinear32(KF);     break;
                    }
                }

                while ((Serializer.BaseStream.Position & 3) != 0) Serializer.BaseStream.WriteByte(0);
            }

            return true;
        }

        internal static H3DFloatKeyFrameGroup ReadGroup(BinaryDeserializer Deserializer, bool Constant)
        {
            H3DFloatKeyFrameGroup FrameGrp = new H3DFloatKeyFrameGroup();

            if (Constant)
            {
                FrameGrp.KeyFrames.Add(new KeyFrame(0, Deserializer.Reader.ReadSingle()));
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

            KeyFrame LHS = KeyFrames.First();
            KeyFrame RHS = KeyFrames.Last();

            foreach (KeyFrame KF in KeyFrames)
            {
                if (KF.Frame <= Frame) LHS = KF;
                if (KF.Frame >= Frame && KF.Frame < RHS.Frame) RHS = KF;
            }

            if (LHS.Frame != RHS.Frame)
            {
                float FrameDiff = Frame - LHS.Frame;
                float Weight    = FrameDiff / (RHS.Frame - LHS.Frame);

                switch (InterpolationType)
                {
                    case H3DInterpolationType.Step: return LHS.Value;
                    case H3DInterpolationType.Linear: return Interpolation.Lerp(LHS.Value, RHS.Value, Weight);
                    case H3DInterpolationType.Hermite:
                        return Interpolation.Herp(
                            LHS.Value,    RHS.Value,
                            LHS.OutSlope, RHS.InSlope,
                            FrameDiff,
                            Weight);

                    default: throw new InvalidOperationException($"Invalid Interpolation type {InterpolationType}!");
                }
            }
            else
            {
                return LHS.Value;
            }
        }
    }
}
