using SPICA.Serialization;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.H3D
{
    class H3DRelocator
    {
        private H3DHeader Header;
        private Stream BaseStream;

        private BinaryReader Reader;
        private BinaryWriter Writer;

        public Dictionary<long, H3DRelocationType> RelocTypes;

        public H3DRelocator(Stream BaseStream, H3DHeader Header)
        {
            this.Header = Header;
            this.BaseStream = BaseStream;

            Reader = new BinaryReader(BaseStream);
            Writer = new BinaryWriter(BaseStream);

            RelocTypes = new Dictionary<long, H3DRelocationType>();
        }

        public void ToAbsolute()
        {
            long Position = BaseStream.Position;

            for (int Offset = 0; Offset < Header.RelocationLength; Offset += 4)
            {
                BaseStream.Seek(Header.RelocationAddress + Offset, SeekOrigin.Begin);

                uint Value = Reader.ReadUInt32();
                uint PtrAddress = Value & 0x1ffffff;

                H3DRelocationType TargetSect = (H3DRelocationType)((Value >> 25) & 0xf);
                H3DRelocationType PointerSect = (H3DRelocationType)(Value >> 29);

                if (TargetSect != H3DRelocationType.Strings) PtrAddress <<= 2;

                Accumulate32(GetAddress(PointerSect) + PtrAddress, GetAddress(TargetSect));
            }

            BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        private uint GetAddress(H3DRelocationType RType)
        {
            switch (RType)
            {
                case H3DRelocationType.Contents: return Header.ContentsAddress;
                case H3DRelocationType.Strings: return Header.StringsAddress;
                case H3DRelocationType.Commands: return Header.CommandsAddress;
                case H3DRelocationType.CommandsSrc: return Header.CommandsAddress;
                case H3DRelocationType.RawData: return Header.RawDataAddress;
                case H3DRelocationType.RawDataTexture: return Header.RawDataAddress;
                case H3DRelocationType.RawDataVertex: return Header.RawDataAddress;
                case H3DRelocationType.RawDataIndex16: return Header.RawDataAddress | (1u << 31);
                case H3DRelocationType.RawDataIndex8: return Header.RawDataAddress;
                case H3DRelocationType.RawExt: return Header.RawExtAddress;
                case H3DRelocationType.RawExtTexture: return Header.RawExtAddress;
                case H3DRelocationType.RawExtVertex: return Header.RawExtAddress;
                case H3DRelocationType.RawExtIndex16: return Header.RawExtAddress | (1u << 31);
                case H3DRelocationType.RawExtIndex8: return Header.RawExtAddress;
            }

            return 0;
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

        public void ToRelative(BinarySerializer Serializer)
        {
            Header.RelocationAddress = (uint)BaseStream.Position;

            foreach (long Pointer in Serializer.Pointers)
            {
                long Position = BaseStream.Position;

                BaseStream.Seek(Pointer, SeekOrigin.Begin);

                uint TargetAddress = Peek32();

                H3DRelocationType Target = GetRelocation(TargetAddress);
                H3DRelocationType Source = GetRelocation(Pointer);

                uint PointerAddress = ToRelative(Pointer, Source);

                Writer.Write(ToRelative(TargetAddress, Target));

                if (RelocTypes.ContainsKey(Pointer)) Target = RelocTypes[Pointer];

                uint Flags;

                Flags = (uint)Target;
                Flags |= (uint)Source << 4;

                if (Target != H3DRelocationType.Strings) PointerAddress >>= 2;

                BaseStream.Seek(Position, SeekOrigin.Begin);

                Writer.Write(PointerAddress | (Flags << 25));
            }

            Header.RelocationLength = (int)(BaseStream.Position - Header.RelocationAddress);
        }

        private H3DRelocationType GetRelocation(long Position)
        {
            if (InRange(Position, Header.ContentsAddress, Header.ContentsLength))
            {
                return H3DRelocationType.Contents;
            }
            else if (InRange(Position, Header.StringsAddress, Header.StringsLength))
            {
                return H3DRelocationType.Strings;
            }
            else if (InRange(Position, Header.CommandsAddress, Header.CommandsLength))
            {
                return H3DRelocationType.Commands;
            }
            else if (InRange(Position, Header.RawDataAddress, Header.RawDataLength))
            {
                return H3DRelocationType.RawData;
            }
            else if (InRange(Position, Header.RawExtAddress, Header.RawExtLength))
            {
                return H3DRelocationType.RawExt;
            }
            else
            {
                return default(H3DRelocationType);
            }
        }

        private uint ToRelative(long Position, H3DRelocationType Relocation)
        {
            switch (Relocation)
            {
                case H3DRelocationType.Contents: return (uint)(Position - Header.ContentsAddress);
                case H3DRelocationType.Strings: return (uint)(Position - Header.StringsAddress);
                case H3DRelocationType.Commands: return (uint)(Position - Header.CommandsAddress);
                case H3DRelocationType.RawData: return (uint)(Position - Header.RawDataAddress);
                case H3DRelocationType.RawExt: return (uint)(Position - Header.RawExtAddress);
            }

            return (uint)Position;
        }

        private bool InRange(long Position, uint Start, int Length)
        {
            return Position >= Start && Position < Start + Length;
        }
    }
}
