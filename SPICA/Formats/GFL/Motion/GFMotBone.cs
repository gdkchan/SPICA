using SPICA.Formats.Common;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL.Motion
{
    class GFMotBone
    {
        public string Name;

        public byte UnkIndex;
        public ushort ParentIndex;
        
        public Vector3D Translation;
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
                    UnkIndex    = Reader.ReadByte(),
                    ParentIndex = Reader.ReadUInt16()
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
                Output[Index].Translation  = new Vector3D(Reader);
                Output[Index].QuatRotation = new Quaternion(Reader);
            }

            return Output;
        }
    }
}
