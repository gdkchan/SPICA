using SPICA.Formats.CtrH3D.Texture;
using SPICA.Formats.GFL2.Utils;

using System.IO;

namespace SPICA.Formats.GFL2.Texture
{
    class GFTexture
    {
        public string Name;
        public byte[] RawBuffer;

        public ushort Width;
        public ushort Height;
        public GFTextureFormat Format;
        public ushort MipmapSize;

        public GFTexture() { }

        public GFTexture(BinaryReader Reader)
        {
            uint MagicNumber = Reader.ReadUInt32();
            uint TextureCount = Reader.ReadUInt32();

            GFSection TextureSection = new GFSection(Reader);

            uint TextureLength = Reader.ReadUInt32();

            Reader.BaseStream.Seek(0xc, SeekOrigin.Current); //Padding? Always zero it seems

            Name = GFString.ReadLength(Reader, 0x40);

            Width = Reader.ReadUInt16();
            Height = Reader.ReadUInt16();
            Format = (GFTextureFormat)Reader.ReadUInt16();
            MipmapSize = Reader.ReadUInt16();

            Reader.BaseStream.Seek(0x10, SeekOrigin.Current); //Padding

            RawBuffer = Reader.ReadBytes((int)TextureLength);
        }

        public H3DTexture ToH3DTexture()
        {
            return new H3DTexture
            {
                Name = Name,
                RawBufferXPos = RawBuffer,
                Width = Width,
                Height = Height,
                Format = Format.ToPICATextureFormat(),
                MipmapSize = (byte)MipmapSize
            };
        }
    }
}
