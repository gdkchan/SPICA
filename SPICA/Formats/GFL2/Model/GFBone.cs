using SPICA.Formats.GFL2.Utils;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Model
{
    public struct GFBone
    {
        public string Name;
        public string ParentName;

        public byte Flags;

        public Vector3D Scale;
        public Vector3D Rotation;
        public Vector3D Translation;

        public GFBone(BinaryReader Reader)
        {
            Name       = GFString.ReadLength(Reader, Reader.ReadByte());
            ParentName = GFString.ReadLength(Reader, Reader.ReadByte());

            Flags = Reader.ReadByte();

            Scale       = new Vector3D(Reader);
            Rotation    = new Vector3D(Reader);
            Translation = new Vector3D(Reader);
        }

        public static List<GFBone> ReadList(BinaryReader Reader, int Count)
        {
            List<GFBone> Output = new List<GFBone>();

            for (int Index = 0; Index < Count; Index++)
            {
                Output.Add(new GFBone(Reader));
            }

            return Output;
        }
    }
}
