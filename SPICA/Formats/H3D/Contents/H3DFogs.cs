using SPICA.Formats.H3D.Contents.Fog;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    class H3DFogs
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("Fogs"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection", 1)]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection", 1), PointerOf("Fogs")]
        private uint[] PointerTable;

        [TargetSection("DescriptorsSection", 4)]
        public H3DFog[] Fogs;

        public H3DFog this[int Index]
        {
            get { return Fogs[Index]; }
            set { Fogs[Index] = value; }
        }
    }
}
