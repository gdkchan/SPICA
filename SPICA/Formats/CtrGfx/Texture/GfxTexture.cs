using SPICA.Formats.Common;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.IO;

namespace SPICA.Formats.CtrGfx.Texture
{
    public class GfxTexture : INamed, ICustomSerialization
    {
        private GfxVersion Revision;

        public bool IsCubeTexture
        {
            get
            {
                return Revision.Inheritance.CheckPath(0, 2);
            }
        }

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value ?? throw Exceptions.GetNullException("Name");
            }
        }

        public readonly GfxDict<GfxMetaData> MetaData;

        public int Height;
        public int Width;

        public uint GLFormat;
        public uint GLType;

        public int MipmapSize;

        private uint TextureObjectPtr;
        private uint LocationFlags;

        public PICATextureFormat HwFormat;

        public GfxTextureImage Image
        {
            get
            {
                return ImageXPos;
            }
            set
            {
                ImageXPos = value;
            }
        }

        [Ignore] public GfxTextureImage ImageXPos;
        [Ignore] public GfxTextureImage ImageXNeg;
        [Ignore] public GfxTextureImage ImageYPos;
        [Ignore] public GfxTextureImage ImageYNeg;
        [Ignore] public GfxTextureImage ImageZPos;
        [Ignore] public GfxTextureImage ImageZNeg;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            long Position = Deserializer.BaseStream.Position;

            for (int i = 0; i < (IsCubeTexture ? 6 : 1); i++)
            {
                Deserializer.BaseStream.Seek(Position + i * 4, SeekOrigin.Begin);
                Deserializer.BaseStream.Seek(Deserializer.ReadPointer(), SeekOrigin.Begin);

                GfxTextureImage Img = Deserializer.Deserialize<GfxTextureImage>();

                switch (i)
                {
                    case 0: ImageXPos = Img; break;
                    case 1: ImageXNeg = Img; break;
                    case 2: ImageYPos = Img; break;
                    case 3: ImageYNeg = Img; break;
                    case 4: ImageZPos = Img; break;
                    case 5: ImageZNeg = Img; break;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //TODO

            return false;
        }
    }
}
