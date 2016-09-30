using SPICA.Serialization;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.H3D
{
    class H3DRelocator : IRelocator
    {
        private H3D Header;
        private Stream BaseStream;

        private BinaryReader Reader;
        private BinaryWriter Writer;

        private struct Pointer
        {
            public long Position;
            public string Hint;
        }

        private struct Section
        {
            public long Position;
            public long Length;
            public string Name;
        }

        private List<Pointer> Pointers;
        private List<Section> Sections;

        public H3DRelocator(H3D Header, Stream BaseStream)
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
            using (MemoryStream MS = new MemoryStream(Header.RelocationTable))
            {
                BinaryReader PtrReader = new BinaryReader(MS);

                while (MS.Position < MS.Length)
                {
                    uint Value = PtrReader.ReadUInt32();

                    uint TargetSec = Value >> 25;
                    uint PtrAddress = Value & 0x1ffffff;

                    //Those values are version specific
                    switch (TargetSec)
                    {
                        case 0: Accumulate32(Header.DescriptorsAddress + (PtrAddress << 2), Header.DescriptorsAddress); break;
                        case 1: Accumulate32(Header.DescriptorsAddress + PtrAddress, Header.StringsAddress); break;
                        case 2: Accumulate32(Header.DescriptorsAddress + (PtrAddress << 2), Header.CommandsAddress); break;
                        case 7: Accumulate32(Header.DescriptorsAddress + (PtrAddress << 2), Header.RawDataAddress); break;
                        case 0x25: Accumulate32(Header.CommandsAddress + (PtrAddress << 2), Header.RawDataAddress); break;
                        case 0x26: Accumulate32(Header.CommandsAddress + (PtrAddress << 2), Header.RawDataAddress); break;
                        case 0x27: Accumulate32(Header.CommandsAddress + (PtrAddress << 2), Header.RawDataAddress | (1u << 31)); break;
                        case 0x28: Accumulate32(Header.CommandsAddress + (PtrAddress << 2), Header.RawDataAddress); break;
                        case 0x2b: Accumulate32(Header.CommandsAddress + (PtrAddress << 2), Header.RawExtAddress); break;
                        case 0x2c: Accumulate32(Header.CommandsAddress + (PtrAddress << 2), Header.RawExtAddress | (1u << 31)); break;
                        case 0x2d: Accumulate32(Header.CommandsAddress + (PtrAddress << 2), Header.RawExtAddress); break;
                    }
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

        public void AddPointer(long Position, string Hint = null)
        {
            Pointers.Add(new Pointer { Position = Position, Hint = Hint });
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
                    uint PointerAddress = (uint)Pointer.Position;

                    Section TargetSect = FindSection(Sects, TargetAddress);
                    Section PointerSect = FindSection(Sects, PointerAddress);

                    Writer.Write((uint)(TargetAddress - TargetSect.Position));

                    uint Flags = 0;

                    if (PointerSect.Name == "CommandsSection")
                    {
                        //TODO
                    }
                    else
                    {
                        switch (TargetSect.Name)
                        {
                            case "DescriptorsSection": Flags = 0; break;
                            case "StringsSection": Flags = 1; break;
                            case "CommandsSection": Flags = 2; break;
                            case "RawDataSection": Flags = 7; break;
                        }
                    }

                    if (TargetSect.Name != "StringsSection") PointerAddress >>= 2;

                    PtrWriter.Write(PointerAddress | (Flags << 25));
                }

                BaseStream.Seek(Position, SeekOrigin.Begin);

                return MS.ToArray();
            }
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
