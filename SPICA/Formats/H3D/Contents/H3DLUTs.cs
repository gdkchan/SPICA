using SPICA.Formats.H3D.Contents.LUT;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    class H3DLUTs
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("LUTs"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection", 1)]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection", 1), PointerOf("LUTs")]
        private uint[] PointerTable;

        [TargetSection("DescriptorsSection", 4)]
        public H3DLUT[] LUTs;

        public H3DLUT this[int Index]
        {
            get { return LUTs[Index]; }
            set { LUTs[Index] = value; }
        }
    }
}
