using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public class GfxTextureReference
    {
        private GfxVersion Revision;

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

        //I think it have more stuff after this...?
    }
}
