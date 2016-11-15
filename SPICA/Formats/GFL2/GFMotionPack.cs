using SPICA.Formats.GFL2.Motion;

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2
{
    class GFMotionPack : IEnumerable<GFMotion>
    {
        private List<GFMotion> Animations;

        public GFMotion this[int Index]
        {
            get { return Animations[Index]; }
            set { Animations[Index] = value; }
        }

        public int Count { get { return Animations.Count; } }

        public GFMotionPack()
        {
            Animations = new List<GFMotion>();
        }

        public GFMotionPack(string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Open))
            {
                GFMotionPackImpl(new BinaryReader(FS));
            }
        }

        public GFMotionPack(Stream Input)
        {
            GFMotionPackImpl(new BinaryReader(Input));
        }

        public GFMotionPack(BinaryReader Reader)
        {
            GFMotionPackImpl(Reader);
        }

        private void GFMotionPackImpl(BinaryReader Reader)
        {
            Animations = new List<GFMotion>();

            uint AnimsCount = Reader.ReadUInt32();

            long Position = Reader.BaseStream.Position;

            for (int Index = 0; Index < AnimsCount; Index++)
            {
                Reader.BaseStream.Seek(Position + Index * 4, SeekOrigin.Begin);

                uint Address = Reader.ReadUInt32();

                if (Address == 0) continue;

                Reader.BaseStream.Seek(Position + Address, SeekOrigin.Begin);

                Animations.Add(new GFMotion(Reader));
            }
        }

        public IEnumerator<GFMotion> GetEnumerator()
        {
            return Animations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
