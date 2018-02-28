using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.GFL2.Model;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFMotion
    {
        private enum Sect
        {
            SubHeader = 0,
            SkeletalAnim = 1,
            MaterialAnim = 3,
            VisibilityAnim = 6
        }

        private struct Section
        {
            public Sect SectName;

            public uint Length;
            public uint Address;
        }

        public uint FramesCount;

        public bool IsLooping;
        public bool IsBlended;

        public Vector3 AnimRegionMin;
        public Vector3 AnimRegionMax;

        public GFSkeletonMot   SkeletalAnimation;
        public GFMaterialMot   MaterialAnimation;
        public GFVisibilityMot VisibilityAnimation;

        public int Index;

        public GFMotion() { }

        public GFMotion(BinaryReader Reader, int Index)
        {
            this.Index = Index;

            long Position = Reader.BaseStream.Position;

            uint MagicNumber  = Reader.ReadUInt32();
            uint SectionCount = Reader.ReadUInt32();

            Section[] AnimSections = new Section[SectionCount];

            for (int Anim = 0; Anim < AnimSections.Length; Anim++)
            {
                AnimSections[Anim] = new Section()
                {
                    SectName = (Sect)Reader.ReadUInt32(),

                    Length  = Reader.ReadUInt32(),
                    Address = Reader.ReadUInt32()
                };
            }

            //SubHeader
            Reader.BaseStream.Seek(Position + AnimSections[0].Address, SeekOrigin.Begin);

            FramesCount = Reader.ReadUInt32();

            IsLooping = (Reader.ReadUInt16() & 1) != 0;
            IsBlended = (Reader.ReadUInt16() & 1) != 0; //Not sure

            AnimRegionMin = Reader.ReadVector3();
            AnimRegionMax = Reader.ReadVector3();

            uint AnimHash = Reader.ReadUInt32();

            //Content
            for (int Anim = 1; Anim < AnimSections.Length; Anim++)
            {
                Reader.BaseStream.Seek(Position + AnimSections[Anim].Address, SeekOrigin.Begin);

                switch (AnimSections[Anim].SectName)
                {
                    case Sect.SkeletalAnim: SkeletalAnimation = new GFSkeletonMot(Reader, FramesCount); break;
                    case Sect.MaterialAnim: MaterialAnimation = new GFMaterialMot(Reader, FramesCount); break;
                    case Sect.VisibilityAnim: VisibilityAnimation = new GFVisibilityMot(Reader, FramesCount); break;
                }
            }
        }

        public H3DAnimation ToH3DSkeletalAnimation(H3DDict<H3DBone> Skeleton)
        {
            List<GFBone> GFSkeleton = new List<GFBone>();

            foreach (H3DBone Bone in Skeleton)
            {
                GFSkeleton.Add(new GFBone()
                {
                    Name        = Bone.Name,
                    Parent      = Bone.ParentIndex != -1 ? Skeleton[Bone.ParentIndex].Name : string.Empty,
                    Flags       = (byte)(Bone.ParentIndex == -1 ? 2 : 1), //TODO: Fix, 2 = Identity and 1 Normal bone?
                    Translation = Bone.Translation,
                    Rotation    = Bone.Rotation,
                    Scale       = Bone.Scale,
                });
            }

            return ToH3DSkeletalAnimation(GFSkeleton);
        }

        public H3DAnimation ToH3DSkeletalAnimation(List<GFBone> Skeleton)
        {
            return SkeletalAnimation?.ToH3DAnimation(Skeleton, this);
        }

        public H3DMaterialAnim ToH3DMaterialAnimation()
        {
            return MaterialAnimation?.ToH3DAnimation(this);
        }

        public H3DAnimation ToH3DVisibilityAnimation()
        {
            return VisibilityAnimation?.ToH3DAnimation(this);
        }
    }
}
