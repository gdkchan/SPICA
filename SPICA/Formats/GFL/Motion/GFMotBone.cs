using SPICA.Formats.Common;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.GFL.Motion
{
    class GFMotBone
    {
        public string Name;

        public byte UnkIndex0;
        public byte ParentIndex;
        public byte UnkIndex1;

        public Vector3    Translation;
        public Quaternion QuatRotation;

        public static List<GFMotBone> ReadSkeleton(BinaryReader Reader)
        {
            List<GFMotBone> Output = new List<GFMotBone>();

            byte BonesCount = Reader.ReadByte();

            Output.Add(new GFMotBone { Name = "Origin" });

            for (int Index = 1; Index < BonesCount; Index++)
            {
                Output.Add(new GFMotBone
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

            GFUtils.Align(Reader);

            for (int Index = 0; Index < BonesCount; Index++)
            {
                Output[Index].Translation  = Reader.ReadVector3();
                Output[Index].QuatRotation = Reader.ReadQuaternion();
            }

            return Output;
        }
    }
}
