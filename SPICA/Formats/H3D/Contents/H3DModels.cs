using SPICA.Formats.H3D.Contents.Model;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    class H3DModels
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("Models"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection", 1)]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection", 1), PointerOf("Models")]
        private uint[] PointerTable;

        [TargetSection("DescriptorsSection", 2)]
        public H3DModel[] Models;

        public H3DModel this[int Index]
        {
            get { return Models[Index]; }
            set { Models[Index] = value; }
        }
    }
}
