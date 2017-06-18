using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [TypeChoice(0x20000004u, typeof(GfxTextureReference))]
    public class GfxTextureReference
    {
        private GfxRevHeader Header;

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

        private string _TextureName;

        public string TextureName
        {
            get
            {
                return _TextureName;
            }
            set
            {
                _TextureName = value ?? throw Exceptions.GetNullException("TextureName");
            }
        }

        private uint TexturePtr;
    }
}
