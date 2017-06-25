using SPICA.Formats.GFL2.Motion;

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2
{
    public class GFMotionPack : IEnumerable<GFMotion>
    {
        private List<GFMotion> Animations;

        public GFMotion this[int Index]
        {
            get => Animations[Index];
            set => Animations[Index] = value;
        }

        public int Count { get { return Animations.Count; } }

        public GFMotionPack()
        {
            Animations = new List<GFMotion>();
        }

        public GFMotionPack(Stream Input) : this(new BinaryReader(Input)) { }

        public GFMotionPack(BinaryReader Reader) : this()
        {
            uint AnimsCount = Reader.ReadUInt32();

            long Position = Reader.BaseStream.Position;

            for (int Index = 0; Index < AnimsCount; Index++)
            {
                Reader.BaseStream.Seek(Position + Index * 4, SeekOrigin.Begin);

                uint Address = Reader.ReadUInt32();

                if (Address == 0) continue;

                Reader.BaseStream.Seek(Position + Address, SeekOrigin.Begin);

                Animations.Add(new GFMotion(Reader, Index));
            }
        }

        public void Add(GFMotion Mot)
        {
            Animations.Add(Mot);
        }

        public void Insert(int Index, GFMotion Mot)
        {
            Animations.Insert(Index, Mot);
        }

        public void Remove(GFMotion Mot)
        {
            Animations.Remove(Mot);
        }

        public void Clear()
        {
            Animations.Clear();
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
