using SPICA.Formats.GFL2.Utils;
using SPICA.Math3D;
using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.GFL2.Model.Material
{
    struct GFTextureCoord
    {
        public uint Hash;
        public string Name;

        public Vector2D Scale;
        public float Rotation;
        public Vector2D Translation;

        public GFTextureWrap WrapU;
        public GFTextureWrap WrapV;

        public GFMagFilter MagFilter;
        public GFMinFilter MinFilter;

        public uint MinLOD;

        public PICATexEnvColor BorderColor;

        public GFTextureCoord(BinaryReader Reader)
        {
            Hash = Reader.ReadUInt32();
            Name = GFString.ReadLength(Reader, Reader.ReadByte());

            Scale = new Vector2D(Reader);
            Rotation = Reader.ReadSingle();
            Translation = new Vector2D(Reader);

            WrapU = (GFTextureWrap)Reader.ReadUInt32();
            WrapV = (GFTextureWrap)Reader.ReadUInt32();

            MagFilter = (GFMagFilter)Reader.ReadUInt32(); //Not sure
            MinFilter = (GFMinFilter)Reader.ReadUInt32(); //Not sure

            MinLOD = Reader.ReadUInt32(); //Not sure

            BorderColor = default(PICATexEnvColor);
        }
    }
}
