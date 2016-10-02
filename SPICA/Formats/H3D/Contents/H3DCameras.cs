using SPICA.Formats.H3D.Contents.Camera;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    class H3DCameras
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("Cameras"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection", 1)]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection", 1), PointerOf("Cameras")]
        private uint[] PointerTable;

        [TargetSection("DescriptorsSection", 4)]
        public H3DCamera[] Cameras;

        public H3DCamera this[int Index]
        {
            get { return Cameras[Index]; }
            set { Cameras[Index] = value; }
        }
    }
}
