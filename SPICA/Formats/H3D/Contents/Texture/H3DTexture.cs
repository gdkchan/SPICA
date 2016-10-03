using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.BinaryAttributes;

using System;
using System.IO;

namespace SPICA.Formats.H3D.Contents.Texture
{
    class H3DTexture : ICustomDeserializer, ICustomSerializer
    {
        [PointerOf("Texture0Commands")]
        private uint Texture0CommandsAddress;

        [CountOf("Texture0Commands")]
        private uint Texture0CommandsCount;

        [PointerOf("Texture1Commands")]
        private uint Texture1CommandsAddress;

        [CountOf("Texture1Commands")]
        private uint Texture1CommandsCount;

        [PointerOf("Texture2Commands")]
        private uint Texture2CommandsAddress;

        [CountOf("Texture2Commands")]
        private uint Texture2CommandsCount;

        public PICATextureFormat Format;
        public byte MipmapSize;
        private ushort Padding;

        [PointerOf("Name")]
        private uint NameAddress;

        [TargetSection("CommandsSection"), CustomSerialization]
        private uint[] Texture0Commands;

        [TargetSection("CommandsSection"), CustomSerialization]
        private uint[] Texture1Commands;

        [TargetSection("CommandsSection"), CustomSerialization]
        private uint[] Texture2Commands;

        [TargetSection("StringsSection")]
        public string Name;

        [TargetSection("RawDataSection")]
        public byte[] RawBuffer;

        [NonSerialized]
        public uint Width;

        [NonSerialized]
        public uint Height;

        public void Deserialize(BinaryDeserializer Deserializer, string FName)
        {
            if (FName != "Texture0Commands") return;

            PICACommandReader Reader = new PICACommandReader(Texture0Commands);

            uint Address = 0;

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_TEXUNIT0_DIM:
                        Height = Param & 0x7ff;
                        Width = (Param >> 16) & 0x7ff;
                        break;
                    case PICARegister.GPUREG_TEXUNIT0_ADDR1: Address = Param; break;
                }
            }

            uint Length = Width * Height;

            switch (Format)
            {
                case PICATextureFormat.RGBA8: Length *= 4; break;
                case PICATextureFormat.RGB8: Length *= 3; break;
                case PICATextureFormat.RGBA5551:
                case PICATextureFormat.RGB565:
                case PICATextureFormat.RGBA4:
                case PICATextureFormat.LA88:
                case PICATextureFormat.HiLo8:
                    Length *= 2;
                    break;
                case PICATextureFormat.L4:
                case PICATextureFormat.A4:
                case PICATextureFormat.ETC1:
                    Length /= 2;
                    break;
            }

            if ((Length & 0x7f) != 0) Length = (Length & ~0x7fu) + 0x80;

            long Position = Deserializer.BaseStream.Position;

            Deserializer.BaseStream.Seek(Address, SeekOrigin.Begin);

            RawBuffer = Deserializer.Reader.ReadBytes((int)Length);

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public object Serialize(BinarySerializer Serializer, string FName)
        {
            PICACommandWriter Writer = new PICACommandWriter();

            switch (FName)
            {
                case "Texture0Commands":
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_DIM, Height | (Width << 16));
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_LOD, MipmapSize);
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_ADDR1, 0);
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_TYPE, (uint)Format);
                    break;

                case "Texture1Commands":
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_DIM, Height | (Width << 16));
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_LOD, MipmapSize);
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_ADDR, 0);
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_TYPE, (uint)Format);
                    break;

                case "Texture2Commands":
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_DIM, Height | (Width << 16));
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_LOD, MipmapSize);
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_ADDR, 0);
                    Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_TYPE, (uint)Format);
                    break;
            }

            Writer.SetCommand(PICARegister.GPUREG_DUMMY, 0, 0);
            Writer.SetCommand(PICARegister.GPUREG_CMDBUF_JUMP1, true);

            Serializer.AddPointer("RawBuffer", this, Serializer.BaseStream.Position + 0x10, typeof(uint));
            Serializer.Relocator.AddPointer(Serializer.BaseStream.Position + 0x10, (int)H3DRelocationType.RawDataTexture);

            return Writer.GetBuffer();
        }
    }
}
