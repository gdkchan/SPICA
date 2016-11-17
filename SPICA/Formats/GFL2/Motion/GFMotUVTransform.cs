using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    class GFMotUVTransform
    {
        public string Name;

        public List<GFMotKeyFrame> ScaleX;
        public List<GFMotKeyFrame> ScaleY;

        public List<GFMotKeyFrame> Rotation;

        public List<GFMotKeyFrame> TranslationX;
        public List<GFMotKeyFrame> TranslationY;

        public GFMotUVTransform()
        {
            InitLists();
        }

        public GFMotUVTransform(BinaryReader Reader, string Name)
        {
            InitLists();

            this.Name = Name;

            Reader.ReadUInt32();

            uint Flags = Reader.ReadUInt32();
            uint Length = Reader.ReadUInt32();

            for (int Elem = 0; Elem < 9; Elem++)
            {
                List<GFMotKeyFrame> KeyFrames = GFMotKeyFrame.ReadList(Reader, Flags);

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

        private void InitLists()
        {
            ScaleX       = new List<GFMotKeyFrame>();
            ScaleY       = new List<GFMotKeyFrame>();

            Rotation     = new List<GFMotKeyFrame>();

            TranslationX = new List<GFMotKeyFrame>();
            TranslationY = new List<GFMotKeyFrame>();
        }
    }
}
