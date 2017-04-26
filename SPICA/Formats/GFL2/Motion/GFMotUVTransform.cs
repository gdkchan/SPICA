using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFMotUVTransform
    {
        public string Name;

        public uint UnitIndex;

        public readonly List<GFMotKeyFrame> ScaleX;
        public readonly List<GFMotKeyFrame> ScaleY;

        public readonly List<GFMotKeyFrame> Rotation;

        public readonly List<GFMotKeyFrame> TranslationX;
        public readonly List<GFMotKeyFrame> TranslationY;

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

            uint Flags  = Reader.ReadUInt32();
            uint Length = Reader.ReadUInt32();

            for (int ElemIndex = 0; ElemIndex < 5; ElemIndex++)
            {
                switch (ElemIndex)
                {
                    case 0: GFMotKeyFrame.SetList(ScaleX,       Reader, Flags, FramesCount); break;
                    case 1: GFMotKeyFrame.SetList(ScaleY,       Reader, Flags, FramesCount); break;

                    case 2: GFMotKeyFrame.SetList(Rotation,     Reader, Flags, FramesCount); break;

                    case 3: GFMotKeyFrame.SetList(TranslationX, Reader, Flags, FramesCount); break;
                    case 4: GFMotKeyFrame.SetList(TranslationY, Reader, Flags, FramesCount); break;
                }

                Flags >>= 3;
            }
        }
    }
}
