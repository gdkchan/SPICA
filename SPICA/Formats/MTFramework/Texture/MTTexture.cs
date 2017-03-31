using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Texture;

using System.IO;

namespace SPICA.Formats.MTFramework.Texture
{
    public class MTTexture
    {
        public byte[] RawBuffer;

        public MTTextureFormat Format;

        public uint Width;
        public uint Height;

        public string Name;

        public MTTexture(BinaryReader Reader, string Name)
        {
            this.Name = Name;

            string Magic = StringUtils.ReadPaddedString(Reader, 4);

            int  Word0 = Reader.ReadInt32();
            uint Word1 = Reader.ReadUInt32();
            uint Word2 = Reader.ReadUInt32();
            uint Padding = Reader.ReadUInt32();

            int  Version = (Word0 >>  0) & 0xfff;
            int  Shift   = (Word0 >> 24) & 0xf;
            uint Width   = (Word1 >>  6) & 0x1fff;
            uint Height  = (Word1 >> 19) & 0x1fff;
            uint Format  = (Word2 >>  8) & 0xff;
            uint Aspect  = (Word2 >> 16) & 0x1fff;

            this.Width  = Width  << Shift;
            this.Height = Height << Shift;

            this.Format = (MTTextureFormat)Format;

            Aspect <<= Shift;

            RawBuffer = Reader.ReadBytes((int)(
                Reader.BaseStream.Length -
                Reader.BaseStream.Position));
        }

        public H3DTexture ToH3DTexture()
        {
            H3DTexture Texture = new H3DTexture();

            Texture.RawBufferXPos = RawBuffer;

            Texture.Name   = Name;
            Texture.Format = Format.ToPICATextureFormat();
            Texture.Width  = Width;
            Texture.Height = Height;

            return Texture;
        }

        public H3D ToH3D()
        {
            H3D Output = new H3D();

            Output.Textures.Add(ToH3DTexture());

            return Output;
        }
    }
}
