using System;
using System.IO;

namespace SPICA.Formats.Common
{
    static class KeyFrameQuantizationHelper
    {
        private const float FP_1_7_8  = 1f / 256;
        private const float FP_1_6_5  = 1f / 32;
        private const float FP_1_10_5 = 1f / 32;

        //Read
        public static KeyFrame ReadHermite128(this BinaryReader Reader)
        {
            return new KeyFrame(
                Reader.ReadSingle(),
                Reader.ReadSingle(),
                Reader.ReadSingle(),
                Reader.ReadSingle());
        }

        public static KeyFrame ReadHermite64(this BinaryReader Reader)
        {
            uint  FrameVal = Reader.ReadUInt32();
            short InSlope  = Reader.ReadInt16();
            short OutSlope = Reader.ReadInt16();

            uint Frame = (FrameVal >>  0) & 0xfff;
            uint Value = (FrameVal >> 12) & 0xfffff;

            return new KeyFrame(
                Frame,
                Value,
                InSlope  * FP_1_7_8,
                OutSlope * FP_1_7_8);
        }

        public static KeyFrame ReadHermite48(this BinaryReader Reader)
        {
            byte   Frame  = Reader.ReadByte();
            ushort Value  = Reader.ReadUInt16();
            uint   Slopes = Reader.ReadUInt24();

            int InSlope  = ((int)Slopes << 20) >> 20;
            int OutSlope = ((int)Slopes <<  8) >> 20;

            return new KeyFrame(
                Frame,
                Value,
                InSlope  * FP_1_6_5,
                OutSlope * FP_1_6_5);
        }

        public static KeyFrame ReadUnifiedHermite96(this BinaryReader Reader)
        {
            return new KeyFrame(
                Reader.ReadSingle(),
                Reader.ReadSingle(),
                Reader.ReadSingle());
        }

        public static KeyFrame ReadUnifiedHermite48(this BinaryReader Reader)
        {
            return new KeyFrame(
                Reader.ReadUInt16() * FP_1_10_5,
                Reader.ReadUInt16(),
                Reader.ReadInt16()  * FP_1_7_8);
        }

        public static KeyFrame ReadUnifiedHermite32(this BinaryReader Reader)
        {
            byte Frame    = Reader.ReadByte();
            uint ValSlope = Reader.ReadUInt24();

            int Value = ((int)ValSlope >> 0) & 0xfff;
            int Slope = ((int)ValSlope << 8) >> 20;

            return new KeyFrame(
                Frame,
                Value,
                Slope * FP_1_6_5);
        }

        public static KeyFrame ReadStepLinear64(this BinaryReader Reader)
        {
            return new KeyFrame(
                Reader.ReadSingle(),
                Reader.ReadSingle());
        }

        public static KeyFrame ReadStepLinear32(this BinaryReader Reader)
        {
            uint FrameVal = Reader.ReadUInt32();

            return new KeyFrame(
                (FrameVal >>  0) & 0xfff,
                (FrameVal >> 12) & 0xfffff);
        }

        //Write
        public static void WriteHermite128(this BinaryWriter Writer, KeyFrame KF)
        {
            Writer.Write(KF.Frame);
            Writer.Write(KF.Value);
            Writer.Write(KF.InSlope);
            Writer.Write(KF.OutSlope);
        }

        public static void WriteHermite64(this BinaryWriter Writer, KeyFrame KF)
        {
            uint FrameVal =
                ((uint)KF.Frame & 0xfff)   <<  0 |
                ((uint)KF.Value & 0xfffff) << 12;

            Writer.Write(FrameVal);
            Writer.Write((short)(KF.InSlope  * 256));
            Writer.Write((short)(KF.OutSlope * 256));
        }

        public static void WriteHermite48(this BinaryWriter Writer, KeyFrame KF)
        {
            int Slopes =
                ((int)(KF.InSlope  * 32) & 0xfff) <<  0 |
                ((int)(KF.OutSlope * 32) & 0xfff) << 12;

            Writer.Write((byte)KF.Frame);
            Writer.Write((ushort)KF.Value);
            Writer.WriteUInt24((uint)Slopes);
        }

        public static void WriteUnifiedHermite96(this BinaryWriter Writer, KeyFrame KF)
        {
            Writer.Write(KF.Frame);
            Writer.Write(KF.Value);
            Writer.Write(KF.InSlope);
        }

        public static void WriteUnifiedHermite48(this BinaryWriter Writer, KeyFrame KF)
        {
            Writer.Write((ushort)(KF.Frame   * 32));
            Writer.Write((ushort)(KF.Value));
            Writer.Write((ushort)(KF.InSlope * 256));
        }

        public static void WriteUnifiedHermite32(this BinaryWriter Writer, KeyFrame KF)
        {
            int ValSlope =
                ((int)(KF.Value)        & 0xfff) <<  0 |
                ((int)(KF.InSlope * 32) & 0xfff) << 12;

            Writer.Write((byte)KF.Frame);
            Writer.WriteUInt24((uint)ValSlope);
        }

        public static void WriteStepLinear64(this BinaryWriter Writer, KeyFrame KF)
        {
            Writer.Write(KF.Frame);
            Writer.Write(KF.Value);
        }

        public static void WriteStepLinear32(this BinaryWriter Writer, KeyFrame KF)
        {
            Writer.Write(
                ((uint)KF.Frame & 0xfff)   <<  0 |
                ((uint)KF.Value & 0xfffff) << 12);
        }

        //Misc.

        /*
         * Frame and Value quantization scales based on Segment quantization.
         * Different encoding modes have different amount of bits available for each field.
         */
        const float q8 = 1f / 0xff;
        const float q12 = 1f / 0xfff;
        const float q16 = 1f / 0xffff;
        const float q20 = 1f / 0xfffff;

        public static float GetFrameScale(KeyFrameQuantization Quantization, float FrameScale = 1)
        {
            switch (Quantization)
            {
                case KeyFrameQuantization.Hermite128:       FrameScale  = 1f;  break;
                case KeyFrameQuantization.Hermite64:        FrameScale *= q12; break;
                case KeyFrameQuantization.Hermite48:        FrameScale *= q8;  break;
                case KeyFrameQuantization.UnifiedHermite96: FrameScale  = 1f;  break;
                case KeyFrameQuantization.UnifiedHermite48: FrameScale  = 1f;  break;
                case KeyFrameQuantization.UnifiedHermite32: FrameScale *= q8;  break;
                case KeyFrameQuantization.StepLinear64:     FrameScale  = 1f;  break;
                case KeyFrameQuantization.StepLinear32:     FrameScale *= q12; break;

                default: throw new InvalidOperationException($"Invalid Segment quantization {Quantization}!");
            }

            return FrameScale;
        }

        public static float GetValueScale(KeyFrameQuantization Quantization, float ValueScale = 1)
        {
            switch (Quantization)
            {
                case KeyFrameQuantization.Hermite128:       ValueScale  = 1f;  break;
                case KeyFrameQuantization.Hermite64:        ValueScale *= q20; break;
                case KeyFrameQuantization.Hermite48:        ValueScale *= q16; break;
                case KeyFrameQuantization.UnifiedHermite96: ValueScale  = 1f;  break;
                case KeyFrameQuantization.UnifiedHermite48: ValueScale *= q16; break;
                case KeyFrameQuantization.UnifiedHermite32: ValueScale *= q12; break;
                case KeyFrameQuantization.StepLinear64:     ValueScale  = 1f;  break;
                case KeyFrameQuantization.StepLinear32:     ValueScale *= q20; break;

                default: throw new InvalidOperationException($"Invalid Segment quantization {Quantization}!");
            }

            return ValueScale;
        }
    }
}
