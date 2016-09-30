using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents.LUT
{
    class H3DLUT
    {
        [PointerOf("Samplers")]
        private uint SamplersAddress;

        [CountOf("Samplers")]
        private uint SamplersCount;

        [PointerOf("Name")]
        private uint NameAddress;

        public H3DLUTSampler[] Samplers;

        [TargetSection("StringsSection")]
        public string Name;
    }
}
