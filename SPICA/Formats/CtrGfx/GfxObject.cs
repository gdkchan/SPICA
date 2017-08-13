using SPICA.Formats.Common;
using SPICA.Formats.CtrGfx.Camera;
using SPICA.Formats.CtrGfx.LUT;
using SPICA.Formats.CtrGfx.Model;
using SPICA.Formats.CtrGfx.Model.Material;
using SPICA.Formats.CtrGfx.Model.Mesh;
using SPICA.Formats.CtrGfx.Texture;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx
{
    [TypeChoice(0x01000000u, typeof(GfxMesh))]
    [TypeChoice(0x02000000u, typeof(GfxSkeleton))]
    [TypeChoice(0x04000000u, typeof(GfxLUT))]
    [TypeChoice(0x08000000u, typeof(GfxMaterial))]
    [TypeChoice(0x10000001u, typeof(GfxShape))]
    [TypeChoice(0x20000004u, typeof(GfxTextureReference))]
    [TypeChoice(0x20000009u, typeof(GfxTextureCube))]
    [TypeChoice(0x20000011u, typeof(GfxTextureImage))]
    [TypeChoice(0x4000000au, typeof(GfxCamera))]
    [TypeChoice(0x40000012u, typeof(GfxModel))]
    [TypeChoice(0x40000092u, typeof(GfxModelSkeletal))]
    [TypeChoice(0x80000001u, typeof(GfxShaderReference))]
    public class GfxObject
    {
        protected GfxRevHeader Header;

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public readonly GfxDict<GfxMetaData> MetaData;

        public GfxObject()
        {
            MetaData = new GfxDict<GfxMetaData>();
        }
    }
}
