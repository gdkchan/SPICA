using SPICA.Formats.H3D.Contents.Animation;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    class H3DAnimations
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("NameTree", 1), CountOf("Animations")]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection")]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection"), PointerOf("Animations")]
        private uint[] PointerTable;

        [TargetSection("DescriptorsSection", 1)]
        public H3DAnimation[] Animations;

        public H3DAnimation this[int Index]
        {
            get { return Animations[Index]; }
            set { Animations[Index] = value; }
        }
    }
}
