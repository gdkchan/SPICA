using SPICA.Formats.Common;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.GFL.Motion
{
    public class GF1MotBone
    {
        public string Name;

        public int  ParentIndex;
        public byte Flags;
        public byte ChildsCount;

        public Vector3    Translation;
        public Quaternion QuatRotation;

        public static List<GF1MotBone> ReadSkeleton(BinaryReader Reader)
        {
            List<GF1MotBone> Output = new List<GF1MotBone>();

            byte BonesCount = Reader.ReadByte();

            //This seems to be the index of the first bone of this Skeleton on the H3D Skeleton.
            //This bone also seems to be maybe used to translate the model around on the world.
            byte FirstSkeletonBoneIndex = Reader.ReadByte();

            Output.Add(new GF1MotBone()
            {
                Name        = "Origin",
                ParentIndex = -1
            });

            for (int Index = 1; Index < BonesCount; Index++)
            {
                Output.Add(new GF1MotBone()
                {
                    ParentIndex = Reader.ReadByte(),
                    Flags       = Reader.ReadByte(),
                    ChildsCount = Reader.ReadByte()
                });
            }

            for (int Index = 1; Index < BonesCount; Index++)
            {
                Output[Index].Name = Reader.ReadNullTerminatedString();
            }

            Reader.Align(4);

            for (int Index = 0; Index < BonesCount; Index++)
            {
                Output[Index].Translation  = Reader.ReadVector3();
                Output[Index].QuatRotation = Reader.ReadQuaternion();
            }

            return Output;
        }
    }
}
