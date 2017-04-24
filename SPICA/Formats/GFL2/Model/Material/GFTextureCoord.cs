using SPICA.Formats.Common;
using SPICA.Math3D;

using System.IO;
using System.Numerics;

namespace SPICA.Formats.GFL2.Model.Material
{
    public struct GFTextureCoord
    {
        public uint   Hash;
        public string Name;

        public GFTextureMappingType MappingType;

        public Vector2 Scale;
        public float   Rotation;
        public Vector2 Translation;

        public GFTextureWrap WrapU;
        public GFTextureWrap WrapV;

        public GFMagFilter MagFilter;
        public GFMinFilter MinFilter;

        public uint MinLOD;

        public GFTextureCoord(BinaryReader Reader)
        {
            Hash = Reader.ReadUInt32();
            Name = Reader.ReadByteLengthString();

            byte UnitIndex = Reader.ReadByte();

            MappingType = (GFTextureMappingType)Reader.ReadByte();

            Scale       = Reader.ReadVector2();
            Rotation    = Reader.ReadSingle();
            Translation = Reader.ReadVector2();

            WrapU = (GFTextureWrap)Reader.ReadUInt32();
            WrapV = (GFTextureWrap)Reader.ReadUInt32();

            MagFilter = (GFMagFilter)Reader.ReadUInt32(); //Not sure
            MinFilter = (GFMinFilter)Reader.ReadUInt32(); //Not sure

            MinLOD = Reader.ReadUInt32(); //Not sure
        }
    }
}
