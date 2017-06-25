using SPICA.Serialization.Attributes;

using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [TypeChoice(0x80000001u, typeof(GfxShaderReference))]
    public class GfxShaderReference
    {
        private GfxRevHeader Header;

        public byte[] ProgramData;

        private uint[] GLShaderKinds;

        public List<GfxShaderDesc> Descriptors;

        private uint[] ShaderObjs; //Same size as above

        private uint CommandCachePtr;
        private uint CommandCacheLength;

        private uint ProgramInfo;
        private uint CommandAlloc;
    }
}
