using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SPICA.Formats.H3D
{
    class H3DRelocator
    {
        private enum RelocationTarget
        {
            DescriptorToDescriptor = 0,
            DescriptorToString = 1,
            DescriptorToCommand = 2
        }

        private Stream BaseStream;
        private BinaryReader Reader;
        private BinaryWriter Writer;

        public H3DRelocator(Stream BaseStream)
        {
            this.BaseStream = BaseStream;

            Reader = new BinaryReader(BaseStream);
            Writer = new BinaryWriter(BaseStream);
        }

        public void ToAbsolute()
        {
            BaseStream.Seek(8, SeekOrigin.Begin);

            uint DescriptorsAddress = Reader.ReadUInt32();
            uint StringsAddress = Reader.ReadUInt32();
            uint CommandsAddress = Reader.ReadUInt32();
            uint RawDataAddress = Reader.ReadUInt32();
            uint RawExtAddress = Reader.ReadUInt32();
            uint RelocationAddress = Reader.ReadUInt32();

            uint DescriptorsLength = Reader.ReadUInt32();
            uint StringsLength = Reader.ReadUInt32();
            uint CommandsLength = Reader.ReadUInt32();
            uint RawDataLength = Reader.ReadUInt32();
            uint RawExtLength = Reader.ReadUInt32();
            uint RelocationLength = Reader.ReadUInt32();

            for (int Offset = 0; Offset < RelocationLength; Offset += 4)
            {
                BaseStream.Seek(RelocationAddress + Offset, SeekOrigin.Begin);

                uint Value = Reader.ReadUInt32();

                uint TargetSec = Value >> 25;
                uint PtrAddress = Value & 0x1ffffff;

                //Those values are version specific
                switch (TargetSec)
                {
                    case 0: Accumulate32(DescriptorsAddress + (PtrAddress << 2), DescriptorsAddress); break;
                    case 1: Accumulate32(DescriptorsAddress + PtrAddress, StringsAddress); break;
                    case 2: Accumulate32(DescriptorsAddress + (PtrAddress << 2), CommandsAddress); break;
                    case 7: Accumulate32(DescriptorsAddress + (PtrAddress << 2), RawDataAddress); break;
                    case 0x25: Accumulate32(CommandsAddress + (PtrAddress << 2), RawDataAddress); break;
                    case 0x26: Accumulate32(CommandsAddress + (PtrAddress << 2), RawDataAddress); break;
                    case 0x27: Accumulate32(CommandsAddress + (PtrAddress << 2), RawDataAddress | (1u << 31)); break;
                    case 0x28: Accumulate32(CommandsAddress + (PtrAddress << 2), RawDataAddress); break;
                    case 0x2b: Accumulate32(CommandsAddress + (PtrAddress << 2), RawExtAddress); break;
                    case 0x2c: Accumulate32(CommandsAddress + (PtrAddress << 2), RawExtAddress | (1u << 31)); break;
                    case 0x2d: Accumulate32(CommandsAddress + (PtrAddress << 2), RawExtAddress); break;
                }
            }
        }

        private void Accumulate32(uint Address, uint Value)
        {
            BaseStream.Seek(Address, SeekOrigin.Begin);
            Value += Peek32();

            Writer.Write(Value);
        }

        private uint Peek32()
        {
            uint Value = Reader.ReadUInt32();
            BaseStream.Seek(-4, SeekOrigin.Current);

            return Value;
        }
    }
}
