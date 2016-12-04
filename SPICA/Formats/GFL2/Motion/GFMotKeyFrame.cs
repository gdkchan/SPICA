using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public struct GFMotKeyFrame
    {
        public byte Frame;
        public float Value;
        public float Slope;

        public GFMotKeyFrame(byte Frame, float Value, float Slope)
        {
            this.Frame = Frame;
            this.Value = Value;
            this.Slope = Slope;
        }

        public static List<GFMotKeyFrame> ReadList(BinaryReader Reader, uint Flags)
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

            return KeyFrames;
        }
    }
}
