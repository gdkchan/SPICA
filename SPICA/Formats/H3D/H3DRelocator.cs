using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.H3D
{
    class H3DRelocator
    {
        private H3DHeader Header;
        private Stream BaseStream;

        private BinaryReader Reader;
        private BinaryWriter Writer;

        private struct Pointer
        {
            public long Position;
            public int Section;
        }

        private struct Section
        {
            public long Position;
            public long Length;
            public string Name;
        }

        private List<Pointer> Pointers;
        private List<Section> Sections;

        public H3DRelocator(Stream BaseStream, H3DHeader Header)
        {
            this.Header = Header;
            this.BaseStream = BaseStream;

            Reader = new BinaryReader(BaseStream);
            Writer = new BinaryWriter(BaseStream);
        }

        public H3DRelocator(Stream BaseStream)
        {
            this.BaseStream = BaseStream;

            Reader = new BinaryReader(BaseStream);
            Writer = new BinaryWriter(BaseStream);

            Pointers = new List<Pointer>();
            Sections = new List<Section>();
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
                case H3DRelocationType.Descriptors: return Header.ContentsAddress;
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

        public void AddPointer(long Position, int Section = -1)
        {
            Pointers.Add(new Pointer { Position = Position, Section = Section });
        }

        public void AddSection(long Position, long Length, string Name)
        {
            Sections.Add(new Section { Position = Position, Length = Length, Name = Name });
        }

        public byte[] GetPointerTable()
        {
            long Position = BaseStream.Position;

            string[] RSectList = new string[]
            {
                "DescriptorsSection",
                "StringsSection",
                "CommandsSection",
                "RawDataSection",
                "RawExtSection"
            };

            IEnumerable<Section> Sects = Sections.Where(Sect => RSectList.Contains(Sect.Name));

            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter PtrWriter = new BinaryWriter(MS);

                foreach (Pointer Pointer in Pointers)
                {
                    Reader.BaseStream.Seek(Pointer.Position, SeekOrigin.Begin);

                    uint TargetAddress = Peek32();

                    Section TargetSect = FindSection(Sects, TargetAddress);
                    Section PointerSect = FindSection(Sects, Pointer.Position);

                    uint PointerAddress = (uint)(Pointer.Position - PointerSect.Position);

                    Writer.Write((uint)(TargetAddress - TargetSect.Position));

                    if (PointerSect.Name != null && TargetAddress != 0)
                    {
                        uint Flags;

                        if (Pointer.Section != -1)
                            Flags = (uint)Pointer.Section;
                        else
                            Flags = (uint)GetRelocationFromName(TargetSect.Name);

                        Flags |= (uint)GetRelocationFromName(PointerSect.Name) << 4;

                        if (TargetSect.Name != "StringsSection") PointerAddress >>= 2;

                        PtrWriter.Write(PointerAddress | (Flags << 25));
                    }
                }

                BaseStream.Seek(Position, SeekOrigin.Begin);

                return MS.ToArray();
            }
        }

        private H3DRelocationType GetRelocationFromName(string SectionName)
        {
            switch (SectionName)
            {
                case "DescriptorsSection": return H3DRelocationType.Descriptors;
                case "StringsSection": return H3DRelocationType.Strings;
                case "CommandsSection": return H3DRelocationType.Commands;
                case "RawDataSection": return H3DRelocationType.RawData;
                case "RawExtSection": return H3DRelocationType.RawExt;
            }

            return default(H3DRelocationType);
        }

        private Section FindSection(IEnumerable<Section> Sects, long Position)
        {
            foreach (Section Sect in Sects)
            {
                long StartPos = Sect.Position;
                long EndPos = StartPos + Sect.Length;

                if (Position >= StartPos && Position < EndPos) return Sect;
            }

            return default(Section);
        }
    }
}
