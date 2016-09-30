using SPICA.Formats.H3D.Contents.Material;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    class H3DMaterials
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("Materials"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection", 1)]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection", 1), PointerOf("Materials")]
        private uint[] PointerTable;

        [TargetSection("DescriptorsSection", 4)]
        public H3DMaterial[] Materials;

        public H3DMaterial this[int Index]
        {
            get { return Materials[Index]; }
            set { Materials[Index] = value; }
        }
    }
}
