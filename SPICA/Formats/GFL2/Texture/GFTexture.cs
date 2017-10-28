using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.PICA.Commands;
using System;
using System.IO;

namespace SPICA.Formats.GFL2.Texture
{
    public class GFTexture
    {
        public string Name;
        public byte[] RawBuffer;

        public ushort Width;
        public ushort Height;
        public GFTextureFormat Format;
        public ushort MipmapSize;

        public GFTexture(H3DTexture tex) {
			Name = tex.Name;
			RawBuffer = tex.RawBuffer;
			Width = (ushort)tex.Width;
			Height = (ushort)tex.Height;
			Format = H3DTextureFormatExtensions.ToGFTextureFormat(tex.Format);
			MipmapSize = tex.MipmapSize;
		}

        public GFTexture(BinaryReader Reader)
        {
            uint MagicNumber = Reader.ReadUInt32();
            uint TextureCount = Reader.ReadUInt32();

            GFSection TextureSection = new GFSection(Reader);

            uint TextureLength = Reader.ReadUInt32();

            Reader.BaseStream.Seek(0xc, SeekOrigin.Current); //Padding? Always zero it seems

            Name = Reader.ReadPaddedString(0x40);

            Width      = Reader.ReadUInt16();
            Height     = Reader.ReadUInt16();
            Format     = (GFTextureFormat)Reader.ReadUInt16();
            MipmapSize = Reader.ReadUInt16();

            Reader.BaseStream.Seek(0x10, SeekOrigin.Current); //Padding

            RawBuffer = Reader.ReadBytes((int)TextureLength);
        }

        public H3DTexture ToH3DTexture()
        {
            return new H3DTexture()
            {
                Name          = Name,
                RawBufferXPos = RawBuffer,
                Width         = Width,
                Height        = Height,
                Format        = Format.ToPICATextureFormat(),
                MipmapSize    = (byte)MipmapSize
            };
        }

		public void Write(BinaryWriter Writer) 
		{
			Writer.Write(0x15041213);
			Writer.Write(1);
			new GFSection("texture", (uint)RawBuffer.Length + 0x68).Write(Writer);
			Writer.Write(RawBuffer.Length);
			Writer.WritePaddedString("", 0x0C);
			Writer.WritePaddedString(Name, 0x40);
			Writer.Write(Width);
			Writer.Write(Height);
			Writer.Write((Int16)Format);
			Writer.Write(MipmapSize);
			Writer.Write(0xFFFFFFFF);
			Writer.Write(0xFFFFFFFF);
			Writer.Write(0xFFFFFFFF);
			Writer.Write(0xFFFFFFFF);
			Writer.Write(RawBuffer);
		}
    }
}
