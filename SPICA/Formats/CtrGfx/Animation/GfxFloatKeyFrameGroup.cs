using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.CtrGfx.Animation
{
    public class GfxFloatKeyFrameGroup : ICustomSerialization
    {
        private float _StartFrame;
        private float _EndFrame;

        public GfxLoopType PreRepeat;
        public GfxLoopType PostRepeat;

        private ushort Padding;

        private uint CurveFlags;
        private uint CurveCount;
        private uint CurveRelPtr;

        public float StartFrame;
        public float EndFrame;

        private uint FormatFlags;

        private int KeysCount;

        private float InvDuration;

        [Ignore] public bool IsLinear;

        [Ignore] public KeyFrameQuantization Quantization;

        [Ignore] public readonly List<KeyFrame> KeyFrames;

        public bool Exists => KeyFrames.Count > 0;

        public GfxFloatKeyFrameGroup()
        {
            KeyFrames = new List<KeyFrame>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Quantization = (KeyFrameQuantization)(FormatFlags >> 5);

            IsLinear = (FormatFlags & 4) != 0;

            float ValueScale  = 1;
            float ValueOffset = 0;
            float FrameScale  = 1;

            if (Quantization != KeyFrameQuantization.Hermite128       &&
                Quantization != KeyFrameQuantization.UnifiedHermite96 &&
                Quantization != KeyFrameQuantization.StepLinear64)
            {
                ValueScale  = Deserializer.Reader.ReadSingle();
                ValueOffset = Deserializer.Reader.ReadSingle();
                FrameScale  = Deserializer.Reader.ReadSingle();
            }

            for (int Index = 0; Index < KeysCount; Index++)
            {
                KeyFrame KF;

                switch (Quantization)
                {
                    case KeyFrameQuantization.Hermite128:       KF = Deserializer.Reader.ReadHermite128();       break;
                    case KeyFrameQuantization.Hermite64:        KF = Deserializer.Reader.ReadHermite64();        break;
                    case KeyFrameQuantization.Hermite48:        KF = Deserializer.Reader.ReadHermite48();        break;
                    case KeyFrameQuantization.UnifiedHermite96: KF = Deserializer.Reader.ReadUnifiedHermite96(); break;
                    case KeyFrameQuantization.UnifiedHermite48: KF = Deserializer.Reader.ReadUnifiedHermite48(); break;
                    case KeyFrameQuantization.UnifiedHermite32: KF = Deserializer.Reader.ReadUnifiedHermite32(); break;
                    case KeyFrameQuantization.StepLinear64:     KF = Deserializer.Reader.ReadStepLinear64();     break;
                    case KeyFrameQuantization.StepLinear32:     KF = Deserializer.Reader.ReadStepLinear32();     break;

                    default: throw new InvalidOperationException($"Invalid Segment quantization {Quantization}!");
                }

                KF.Frame = KF.Frame * FrameScale;
                KF.Value = KF.Value * ValueScale + ValueOffset;

                KeyFrames.Add(KF);
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            if (Exists)
            {
                KeysCount = KeyFrames.Count;

                float MinFrame = KeyFrames[0].Frame;
                float MaxFrame = KeyFrames[0].Frame;
                float MinValue = KeyFrames[0].Value;
                float MaxValue = KeyFrames[0].Value;

                for (int Index = 1; Index < KeysCount; Index++)
                {
                    KeyFrame KF = KeyFrames[Index];

                    if (KF.Frame < MinFrame) MinFrame = KF.Frame;
                    if (KF.Frame > MaxFrame) MaxFrame = KF.Frame;
                    if (KF.Value < MinValue) MinValue = KF.Value;
                    if (KF.Value > MaxValue) MaxValue = KF.Value;
                }

                float ValueScale  = KeyFrameQuantizationHelper.GetValueScale(Quantization, MaxValue - MinValue);
                float FrameScale  = KeyFrameQuantizationHelper.GetValueScale(Quantization, MaxFrame - MinFrame);

                float ValueOffset = MinValue;

                InvDuration = 1f / EndFrame;

                if (ValueScale == 1)
                {
                    /*
                     * Quantizations were the value scale is not needed (like the ones that already stores the value
                     * as float) will ignore the offset aswell, so we need to set to to zero.
                     */
                    ValueOffset = 0;
                }

                _StartFrame = StartFrame;
                _EndFrame   = EndFrame;

                CurveFlags  = KeyFrames.Count == 0 ? 2u : 4u;
                CurveCount  = 1;
                CurveRelPtr = 4;

                FormatFlags = ((uint)Quantization << 5) | (KeyFrames.Count == 1 ? 1u : 0u);

                if (Quantization >= KeyFrameQuantization.StepLinear64)
                {
                    FormatFlags |= IsLinear ? 4u : 0u;
                }
                else
                {
                    FormatFlags |= 8;
                }

                Serializer.WriteValue(this);

                if (Quantization != KeyFrameQuantization.Hermite128       &&
                    Quantization != KeyFrameQuantization.UnifiedHermite96 &&
                    Quantization != KeyFrameQuantization.StepLinear64)
                {
                    Serializer.Writer.Write(ValueScale);
                    Serializer.Writer.Write(ValueOffset);
                    Serializer.Writer.Write(FrameScale);
                }

                foreach (KeyFrame Key in KeyFrames)
                {
                    KeyFrame KF = Key;

                    KF.Frame = (KF.Frame / FrameScale);
                    KF.Value = (KF.Value - ValueOffset) / ValueScale;                   

                    switch (Quantization)
                    {
                        case KeyFrameQuantization.Hermite128:       Serializer.Writer.WriteHermite128(KF);       break;
                        case KeyFrameQuantization.Hermite64:        Serializer.Writer.WriteHermite64(KF);        break;
                        case KeyFrameQuantization.Hermite48:        Serializer.Writer.WriteHermite48(KF);        break;
                        case KeyFrameQuantization.UnifiedHermite96: Serializer.Writer.WriteUnifiedHermite96(KF); break;
                        case KeyFrameQuantization.UnifiedHermite48: Serializer.Writer.WriteUnifiedHermite48(KF); break;
                        case KeyFrameQuantization.UnifiedHermite32: Serializer.Writer.WriteUnifiedHermite32(KF); break;
                        case KeyFrameQuantization.StepLinear64:     Serializer.Writer.WriteStepLinear64(KF);     break;
                        case KeyFrameQuantization.StepLinear32:     Serializer.Writer.WriteStepLinear32(KF);     break;
                    }
                }

                while ((Serializer.BaseStream.Position & 3) != 0) Serializer.BaseStream.WriteByte(0);
            }

            return true;
        }

        internal static GfxFloatKeyFrameGroup ReadGroup(BinaryDeserializer Deserializer, bool Constant)
        {
            GfxFloatKeyFrameGroup FrameGrp = new GfxFloatKeyFrameGroup();

            if (Constant)
            {
                FrameGrp.KeyFrames.Add(new KeyFrame(0, Deserializer.Reader.ReadSingle()));
            }
            else
            {
                uint Address = Deserializer.ReadPointer();

                Deserializer.BaseStream.Seek(Address, SeekOrigin.Begin);

                FrameGrp = Deserializer.Deserialize<GfxFloatKeyFrameGroup>();
            }

            return FrameGrp;
        }
    }
}
