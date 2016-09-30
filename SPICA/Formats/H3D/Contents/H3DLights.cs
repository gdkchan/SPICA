using SPICA.Formats.H3D.Contents.Light;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    class H3DLights
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("Lights"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection", 1)]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection", 1), PointerOf("Lights")]
        private uint[] PointerTable;

        [TargetSection("DescriptorsSection", 4)]
        public H3DLight[] Lights;

        public H3DLight this[int Index]
        {
            get { return Lights[Index]; }
            set { Lights[Index] = value; }
        }
    }
}
