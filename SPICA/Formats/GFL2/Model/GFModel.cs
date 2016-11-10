using SPICA.Formats.GFL2.Model.Material;
using SPICA.Formats.GFL2.Utils;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.GFL2.Model
{
    class GFModel
    {
        public Vector4D BBoxMinVector;
        public Vector4D BBoxMaxVector;

        public Matrix4x4 Transform;

        public List<GFBone>     Skeleton;
        public List<GFLUT>      LUTs;
        public List<GFMaterial> Materials;

        public GFModel()
        {
            Skeleton  = new List<GFBone>();
            LUTs      = new List<GFLUT>();
            Materials = new List<GFMaterial>();
        }

        public GFModel(BinaryReader Reader)
        {
            GFSection ModelSection = new GFSection(Reader);

            GFHashName[] EffectNames   = ReadStringTable(Reader);
            GFHashName[] LUTNames      = ReadStringTable(Reader);
            GFHashName[] MaterialNames = ReadStringTable(Reader);
            GFHashName[] MeshNames     = ReadStringTable(Reader);

            BBoxMinVector = new Vector4D(Reader);
            BBoxMaxVector = new Vector4D(Reader);

            Transform = new Matrix4x4(Reader);

            //TODO: Investigate what is this (maybe Tile permissions?)
            uint UnknownDataLength = Reader.ReadUInt32();
            uint UnknownDataOffset = Reader.ReadUInt32();
            ulong Padding = Reader.ReadUInt64();

            Reader.BaseStream.Seek(UnknownDataOffset + UnknownDataLength, SeekOrigin.Current);

            uint BonesCount = Reader.ReadUInt32();

            GFSection.SkipPadding(Reader);

            Skeleton = new List<GFBone>();

            for (int Index = 0; Index < BonesCount; Index++)
            {
                Skeleton.Add(new GFBone(Reader));
            }

            GFSection.SkipPadding(Reader);

            uint LUTsCount = Reader.ReadUInt32();
            int LUTLength = Reader.ReadInt32();

            GFSection.SkipPadding(Reader);

            LUTs = new List<GFLUT>();

            for (int Index = 0; Index < LUTsCount; Index++)
            {
                LUTs.Add(new GFLUT(Reader, LUTNames[Index].Name, LUTLength));
            }

            Materials = new List<GFMaterial>();


        }

        private GFHashName[] ReadStringTable(BinaryReader Reader)
        {
            uint Count = Reader.ReadUInt32();

            GFHashName[] Values = new GFHashName[Count];

            for (int Index = 0; Index < Count; Index++)
            {
                Values[Index].Hash = Reader.ReadUInt32();
                Values[Index].Name = GFString.ReadLength(Reader, 0x40);
            }

            return Values;
        }

        public void Write(BinaryWriter Writer)
        {
            //TODO
        }

    }
}
