using SPICA.Formats.Common;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.GFL.Motion
{
    class GF1MotBone
    {
        public string Name;

        public byte UnkIndex0;
        public byte ParentIndex;
        public byte UnkIndex1;

        public Vector3    Translation;
        public Quaternion QuatRotation;

        public static List<GF1MotBone> ReadSkeleton(BinaryReader Reader)
        {
            List<GF1MotBone> Output = new List<GF1MotBone>();

            byte BonesCount = Reader.ReadByte();

            Output.Add(new GF1MotBone { Name = "Origin" });

            for (int Index = 1; Index < BonesCount; Index++)
            {
                Output.Add(new GF1MotBone
                {
                    UnkIndex0   = Reader.ReadByte(),
                    ParentIndex = Reader.ReadByte(),
                    UnkIndex1   = Reader.ReadByte()
                });
            }

            //This usually have the value one, no idea of what it is supposed to be, maybe padding?
            Reader.ReadByte();

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
