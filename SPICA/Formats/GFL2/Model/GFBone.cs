using SPICA.Formats.GFL2.Utils;
using SPICA.Math3D;

using System.IO;

namespace SPICA.Formats.GFL2.Model
{
    struct GFBone
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
    }
}
