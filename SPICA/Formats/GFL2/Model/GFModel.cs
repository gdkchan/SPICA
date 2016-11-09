using SPICA.Formats.GFL2.Utils;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.GFL2.Model
{
    class GFModel
    {
        public List<GFHashName> EffectNames;
        public List<GFHashName> LUTNames;
        public List<GFHashName> MaterialNames;
        public List<GFHashName> MeshNames;

        public Vector4D BBoxMinVector;
        public Vector4D BBoxMaxVector;

        public Matrix4x4 Transform;

        public List<GFBone>     Skeleton;
        public List<GFLUT>      LUTs;
        public List<GFMaterial> Materials;

        public GFModel()
        {
            EffectNames   = new List<GFHashName>();
            LUTNames      = new List<GFHashName>();
            MaterialNames = new List<GFHashName>();
            MeshNames     = new List<GFHashName>();

            Skeleton  = new List<GFBone>();
            LUTs      = new List<GFLUT>();
            Materials = new List<GFMaterial>();
        }

        public GFModel(BinaryReader Reader)
        {
            GFSection ModelSection = new GFSection(Reader);

            EffectNames   = ReadStringTable(Reader).ToList();
            LUTNames      = ReadStringTable(Reader).ToList();
            MaterialNames = ReadStringTable(Reader).ToList();
            MeshNames     = ReadStringTable(Reader).ToList();

            BBoxMinVector = new Vector4D(Reader);
            BBoxMaxVector = new Vector4D(Reader);

            Transform = new Matrix4x4(Reader);

            //TODO: Investigate what is this (maybe Tile permissions?)
            uint UnknownDataLength = Reader.ReadUInt32();
            uint UnknownDataOffset = Reader.ReadUInt32();
            ulong Padding = Reader.ReadUInt64();

            Reader.BaseStream.Seek(UnknownDataOffset + UnknownDataLength, SeekOrigin.Current);

            uint BonesCount = Reader.ReadUInt32();

            Reader.BaseStream.Seek(0xc, SeekOrigin.Current); //Padding

            Skeleton = new List<GFBone>();

            for (int Index = 0; Index < BonesCount; Index++)
            {
                Skeleton.Add(new GFBone(Reader));
            }

            GFSection.SkipPadding(Reader);

            uint LUTsCount = Reader.ReadUInt32();
            int LUTLength = Reader.ReadInt32();

            Reader.BaseStream.Seek(8, SeekOrigin.Current); //Padding

            LUTs = new List<GFLUT>();

            for (int Index = 0; Index < LUTsCount; Index++)
            {
                LUTs.Add(new GFLUT(Reader, LUTNames[Index].Name, LUTLength));
            }

            Materials = new List<GFMaterial>();


        }

        public void Write(BinaryWriter Writer)
        {

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

        private void WriteStringTable(BinaryWriter Writer, IEnumerable<GFHashName> Values)
        {
            Writer.Write(Values.Count());

            foreach (GFHashName Value in Values)
            {
                Writer.Write(Value.Hash);
                GFString.WriteLength(Writer, Value.Name, 0x40);
            }
        }
    }
}
