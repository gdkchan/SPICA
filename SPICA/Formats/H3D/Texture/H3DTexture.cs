using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;

using System;
using System.IO;

namespace SPICA.Formats.H3D.Texture
{
    class H3DTexture : ICustomSerialization, ICustomSerializeCmd
    {
        public uint[] Texture0Commands;
        public uint[] Texture1Commands;
        public uint[] Texture2Commands;

        public PICATextureFormat Format;
        public byte MipmapSize;
        public ushort Padding;

        public string Name;

        [NonSerialized]
        public byte[] RawBuffer;

        [NonSerialized]
        public uint Width;

        [NonSerialized]
        public uint Height;

        public void Deserialize(BinaryDeserializer Deserializer)
        {
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

        public bool Serialize(BinarySerializer Serializer)
        {
            for (int Unit = 0; Unit < 3; Unit++)
            {
                PICACommandWriter Writer = new PICACommandWriter();

                switch (Unit)
                {
                    case 0:
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_DIM, Height | (Width << 16));
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_LOD, MipmapSize);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_ADDR1, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_TYPE, (uint)Format);
                        break;

                    case 1:
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_DIM, Height | (Width << 16));
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_LOD, MipmapSize);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_ADDR, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_TYPE, (uint)Format);
                        break;

                    case 2:
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_DIM, Height | (Width << 16));
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_LOD, MipmapSize);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_ADDR, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_TYPE, (uint)Format);
                        break;
                }

                Writer.SetCommand(PICARegister.GPUREG_DUMMY, 0, 0);
                Writer.SetCommand(PICARegister.GPUREG_CMDBUF_JUMP1, true);

                switch (Unit)
                {
                    case 0: Texture0Commands = Writer.GetBuffer(); break;
                    case 1: Texture1Commands = Writer.GetBuffer(); break;
                    case 2: Texture2Commands = Writer.GetBuffer(); break;
                }
            }

            return false;
        }

        public void SerializeCmd(BinarySerializer Serializer, object Value)
        {
            long Position = Serializer.BaseStream.Position + 0x10;

            Serializer.RawDataTex.Values.Add(new BinarySerializer.RefValue
            {
                Value = RawBuffer,
                Position = Position
            });

            Serializer.Relocator.RelocTypes.Add(Position, H3DRelocationType.RawDataTexture);
        }
    }
}
