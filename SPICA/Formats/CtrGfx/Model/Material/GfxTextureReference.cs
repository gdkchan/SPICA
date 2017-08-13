using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [TypeChoice(0x20000004u, typeof(GfxTextureReference))]
    public class GfxTextureReference : GfxObject
    {
        private string _Path;

        public string Path
        {
            get => _Path;
            set => _Path = value ?? throw Exceptions.GetNullException("Path");
        }

        private uint TexturePtr;
    }
}
