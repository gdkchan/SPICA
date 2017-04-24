using SPICA.Formats.GFL.Motion;

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL
{
    class GFMotionPack : IEnumerable<GFMotion>
    {
        private List<GFMotion> Animations;

        public GFMotion this[int Index]
        {
            get
            {
                return Animations[Index];
            }
            set
            {
                Animations[Index] = value;
            }
        }

        public int Count { get { return Animations.Count; } }

        public GFMotionPack()
        {
            Animations = new List<GFMotion>();
        }

        public GFMotionPack(Stream Input) : this(new BinaryReader(Input)) { }

        public GFMotionPack(BinaryReader Reader) : this()
        {
            long Position = Reader.BaseStream.Position;

            uint AnimsCount = Reader.ReadUInt32();

            Reader.BaseStream.Seek(Reader.ReadUInt32() - 8, SeekOrigin.Current);

            List<GFMotBone> Skeleton = GFMotBone.ReadSkeleton(Reader);

            for (int Index = 1; Index < AnimsCount; Index++)
            {
                Reader.BaseStream.Seek(Position + 4 + Index * 4, SeekOrigin.Begin);

                uint Address = Reader.ReadUInt32();

                if (Address == 0) continue;

                Reader.BaseStream.Seek(Position + Address, SeekOrigin.Begin);

                Animations.Add(new GFMotion(Reader, Skeleton, Index - 1));
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
