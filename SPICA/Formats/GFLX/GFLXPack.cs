using K4os.Compression.LZ4;
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
                    Names.Add(offset.ToString("X8"));
                    br.BaseStream.Position = (long)offset;
                    byte[] compData = br.ReadBytes((int)zsize);
                    byte[] decompData = new byte[size];
                    var decoded = LZ4Codec.Decode(compData, 0, compData.Length, decompData, 0, decompData.Length);
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
    }
}
