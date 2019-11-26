using SPICA.Compression;
using SPICA.Formats.CtrH3D;
using System;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFLX
{
    public class GFLXPack
    {
        public string Magic;
        public UInt32 FileCnt;
        UInt64 InfoOff;

        List<byte[]> Files;
        List<string> Names;

        public GFLXPack(BinaryReader br) {
            //
        }

        public GFLXPack(string path)
        {
            Files = new List<byte[]>();
            Names = new List<string>();
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                //Read container header
                Magic = br.ReadChars(8).ToString();
                br.ReadUInt64();
                FileCnt = br.ReadUInt32();
                br.ReadUInt32();
                InfoOff = br.ReadUInt64();
                br.ReadUInt64();
                br.ReadUInt64();
                br.ReadUInt64();

                for (int i = 0; i < FileCnt; i++)
                {
                    br.BaseStream.Position = (long)InfoOff + (i * 0x18);
                    //Read file header
                    br.ReadUInt32();
                    UInt32 size = br.ReadUInt32();
                    UInt32 zsize = br.ReadUInt32();
                    br.ReadUInt32(); //dummy
                    UInt64 offset = br.ReadUInt64();
                    br.BaseStream.Position = (long)offset;
                    byte[] compData = br.ReadBytes((int)zsize);
                    byte[] decompData = LZ4.Decompress(compData, (int)size);
                    string ext = string.Empty;
                    switch (BitConverter.ToUInt32(decompData, 0))
                    {
                        case 0x58544E42:
                            ext = ".btnx";
                            break;
                        case 0x48534E42:
                            ext = ".bnsh";
                            break;
                        case 0x20:
                            ext = ".gfbmdl";
                            break;
                        default:
                            ext = ".bin";
                            break;
                    }
                    Names.Add(offset.ToString("X8") + ext);
                    Files.Add(decompData);
                }
            }
        }

        ~GFLXPack()
        {
            //
        }

        public byte[] GetFile(int ind)
        {
            if (ind >= FileCnt) return null;
            return Files[ind];
        }

        public string GetName(int ind) {
            if (ind >= FileCnt) return string.Empty;
            return Names[ind];
        }

        public H3D ToH3D()
        {
            H3D h3d = new H3D();
            return h3d;
        }
    }
}
