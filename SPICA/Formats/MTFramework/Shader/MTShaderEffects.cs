using SPICA.Formats.Common;

using System;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.MTFramework.Shader
{
    public class MTShaderEffects
    {
        private
            Dictionary<uint, object> Descriptors = new
            Dictionary<uint, object>();

        public MTShaderEffects(Stream Input) : this(new BinaryReader(Input)) { }

        public MTShaderEffects(BinaryReader Reader)
        {
            string Magic = Reader.ReadNullTerminatedString(); //MFX

            Reader.BaseStream.Seek(8, SeekOrigin.Current); //Unknown stuff

            uint DescriptorsCount = Reader.ReadUInt32();
            uint FragShaderCount  = Reader.ReadUInt32();
            uint VtxShaderCount   = Reader.ReadUInt32();
            uint FragShaderAddr   = Reader.ReadUInt32();
            uint VtxShaderAddr    = Reader.ReadUInt32();
            uint StringsTblAddr   = Reader.ReadUInt32();
            uint VtxProgramAddr   = Reader.ReadUInt32();

            long VtxFormatsAddr = Reader.BaseStream.Position;

            System.Diagnostics.Debug.WriteLine(DescriptorsCount);

            for (uint i = 0; i < DescriptorsCount; i++)
            {
                Reader.BaseStream.Seek(VtxFormatsAddr + i * 4, SeekOrigin.Begin);
                Reader.BaseStream.Seek(Reader.ReadUInt32(), SeekOrigin.Begin);

                if (Reader.BaseStream.Position == 0) continue;

                string DescName = GetName(Reader, StringsTblAddr);
                string TypeName = GetName(Reader, StringsTblAddr);

                ushort DescType   = Reader.ReadUInt16();
                ushort MapLength  = Reader.ReadUInt16(); //Actual length is value / 2? Not sure
                ushort MapIndex   = Reader.ReadUInt16();
                ushort DescIndex  = Reader.ReadUInt16();
                uint   MapAddress = Reader.ReadUInt32(); //Not sure what this address actually points to

                uint Hash = (CRC32Hash.Hash(DescName) << 12) | DescIndex;

                System.Diagnostics.Debug.WriteLine(DescName + " - " + Hash.ToString("x8"));

                object Desc = null;

                if (TypeName == "__InputLayout")
                {
                    Desc = new MTAttributesGroup(Reader, StringsTblAddr);
                }
                else
                {
                    switch (DescType)
                    {
                        case 5: Desc = new MTAlphaBlendConfig(Reader); break;
                        case 6: Desc = new MTDepthStencilConfig(Reader); break;
                    }
                }

                if (Desc != null) Descriptors.Add(Hash, Desc);
            }
        }

        internal static string GetName(BinaryReader Reader, uint StringsAddr)
        {
            uint Address = Reader.ReadUInt32();

            long Position = Reader.BaseStream.Position;

            Reader.BaseStream.Seek(StringsAddr + Address, SeekOrigin.Begin);

            string Output = Reader.ReadNullTerminatedString();

            Reader.BaseStream.Seek(Position, SeekOrigin.Begin);

            return Output;
        }

        public T GetDescriptor<T>(uint HashIndex)
        {
            object Value;

            bool Found = Descriptors.TryGetValue(HashIndex, out Value);

            if (!Found)
            {
                string CRC = (HashIndex >> 12).ToString("x5");
                string Idx = (HashIndex & 0xfff).ToString();

                throw new ArgumentException($"Hash/Index not found (CRC32: ???{CRC}, Index: {Idx})");
            }

            return Found ? (T)Value : default(T);
        }
    }
}
