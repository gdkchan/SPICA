using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFMotUVTransform
    {
        public string Name;
        public uint UnitIndex;

        public List<GFMotKeyFrame> ScaleX;
        public List<GFMotKeyFrame> ScaleY;

        public List<GFMotKeyFrame> Rotation;

        public List<GFMotKeyFrame> TranslationX;
        public List<GFMotKeyFrame> TranslationY;

        public GFMotUVTransform()
        {
            ScaleX       = new List<GFMotKeyFrame>();
            ScaleY       = new List<GFMotKeyFrame>();

            Rotation     = new List<GFMotKeyFrame>();

            TranslationX = new List<GFMotKeyFrame>();
            TranslationY = new List<GFMotKeyFrame>();
        }

        public GFMotUVTransform(BinaryReader Reader, string Name, uint FramesCount) : this()
        {
            this.Name = Name;

            UnitIndex = Reader.ReadUInt32();

            uint Flags = Reader.ReadUInt32();
            uint Length = Reader.ReadUInt32();

            for (int Elem = 0; Elem < 5; Elem++)
            {
                List<GFMotKeyFrame> KeyFrames = GFMotKeyFrame.ReadList(Reader, Flags, FramesCount);

                switch (Elem)
                {
                    case 0: ScaleX       = KeyFrames; break;
                    case 1: ScaleY       = KeyFrames; break;

                    case 2: Rotation     = KeyFrames; break;

                    case 3: TranslationX = KeyFrames; break;
                    case 4: TranslationY = KeyFrames; break;
                }

                Flags >>= 3;
            }
        }
    }
}
