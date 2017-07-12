using SPICA.Formats.GFL.Motion;

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL
{
    public class GF1MotionPack : IEnumerable<GF1Motion>
    {
        private List<GF1Motion> Animations;

        public GF1Motion this[int Index]
        {
            get => Animations[Index];
            set => Animations[Index] = value;
        }

        public int Count { get { return Animations.Count; } }

        public GF1MotionPack()
        {
            Animations = new List<GF1Motion>();
        }

        public GF1MotionPack(Stream Input) : this(new BinaryReader(Input)) { }

        public GF1MotionPack(BinaryReader Reader) : this()
        {
            long Position = Reader.BaseStream.Position;

            uint AnimsCount = Reader.ReadUInt32();

            Reader.BaseStream.Seek(Reader.ReadUInt32() - 8, SeekOrigin.Current);

            List<GF1MotBone> Skeleton = GF1MotBone.ReadSkeleton(Reader);

            for (int Index = 1; Index < AnimsCount; Index++)
            {
                Reader.BaseStream.Seek(Position + 4 + Index * 4, SeekOrigin.Begin);

                uint Address = Reader.ReadUInt32();

                if (Address == 0) continue;

                Reader.BaseStream.Seek(Position + Address, SeekOrigin.Begin);

                Animations.Add(new GF1Motion(Reader, Skeleton, Index - 1));
            }
        }

        public IEnumerator<GF1Motion> GetEnumerator()
        {
            return Animations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
