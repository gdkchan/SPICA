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

        public List<GFSkeletonMot> SkeletalAnimations;

        public GFMotion()
        {
            SkeletalAnimations = new List<GFSkeletonMot>();
        }

        public GFMotion(BinaryReader Reader)
        {
            SkeletalAnimations = new List<GFSkeletonMot>();

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
                    AnimSections[Anim].Count = Reader.ReadUInt32();
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
            for (int Anim = 0; Anim < 1; Anim++)
            {
                Reader.BaseStream.Seek(Position + AnimSections[Anim].Address, SeekOrigin.Begin);

                for (int Index = 0; Index < AnimSections[Anim].Count; Index++)
                {
                    switch (Anim)
                    {
                        case 0: SkeletalAnimations.Add(new GFSkeletonMot(Reader)); break;
                    }
                }
            }
        }

        public PatriciaList<H3DAnimation> ToH3DSkeletalAnimationList(List<GFBone> Skeleton)
        {
            PatriciaList<H3DAnimation> Output = new PatriciaList<H3DAnimation>();

            for (int Index = 0; Index < SkeletalAnimations.Count; Index++)
            {
                H3DAnimation Anim = SkeletalAnimations[Index].ToH3DAnimation(Skeleton, FramesCount);

                Anim.Name = $"SklMot_{Index}";

                Output.Add(Anim);
            }

            return Output;
        }
    }
}
