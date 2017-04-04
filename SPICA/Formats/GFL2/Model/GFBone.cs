using SPICA.Formats.Common;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Model
{
    public struct GFBone
    {
        public string Name;
        public string Parent;
        public byte   Flags;

        public Vector3D Scale;
        public Vector3D Rotation;
        public Vector3D Translation;

        public GFBone(BinaryReader Reader)
        {
            Name   = Reader.ReadByteLengthString();
            Parent = Reader.ReadByteLengthString();
            Flags  = Reader.ReadByte();

            Scale       = new Vector3D(Reader);
            Rotation    = new Vector3D(Reader);
            Translation = new Vector3D(Reader);
        }
    }
}
