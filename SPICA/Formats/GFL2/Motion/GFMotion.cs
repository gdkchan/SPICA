using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.GFL2.Model;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    class GFMotion
    {
        private struct Section
        {
            public bool Exists;

            public uint Count;
            public uint Length;
            public uint Address;
        }

        public Vector3D AnimRegionMin;
        public Vector3D AnimRegionMax;

        public uint FramesCount;

        public GFSkeletonMot SkeletalAnimation;
        public GFMaterialMot MaterialAnimation;

        public GFMotion() { }

        public GFMotion(BinaryReader Reader)
        {
            long Position = Reader.BaseStream.Position;

            uint MagicNumber = Reader.ReadUInt32();
            uint AnimContentFlags = Reader.ReadUInt32();
            Reader.ReadUInt32();
            Reader.ReadUInt32();
            uint SubHeaderAddress = Reader.ReadUInt32();

            Section[] AnimSections = new Section[2];

            for (int Anim = 0; Anim < 2; Anim++)
            {
                AnimSections[Anim].Exists = (AnimContentFlags & (2 >> Anim)) != 0;

                if (AnimSections[Anim].Exists)
                {
                    AnimSections[Anim].Count = Reader.ReadUInt32(); //TODO: Fix this, not a count it seems
                    AnimSections[Anim].Length = Reader.ReadUInt32();
                    AnimSections[Anim].Address = Reader.ReadUInt32();
                }
            }

            //SubHeader
            FramesCount = Reader.ReadUInt32();

            Reader.ReadUInt32();

            AnimRegionMin = new Vector3D(Reader);
            AnimRegionMax = new Vector3D(Reader);

            uint AnimHash = Reader.ReadUInt32();

            //Content
            for (int Anim = 0; Anim < 2; Anim++)
            {
                Reader.BaseStream.Seek(Position + AnimSections[Anim].Address, SeekOrigin.Begin);

                if (!AnimSections[Anim].Exists) continue;

                switch (Anim)
                {
                    case 0: SkeletalAnimation = new GFSkeletonMot(Reader); break;
                    case 1: MaterialAnimation = new GFMaterialMot(Reader); break;
                }
            }
        }

        public H3DAnimation ToH3DSkeletalAnimation(List<GFBone> Skeleton)
        {
            return SkeletalAnimation.ToH3DAnimation(Skeleton, FramesCount);
        }

        public H3DAnimation ToH3DMaterialAnimation()
        {
            return MaterialAnimation.ToH3DAnimation(FramesCount);
        }
    }
}
