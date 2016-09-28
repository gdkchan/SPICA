using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    struct H3DTreeNode
    {
        public int ReferenceBit;
        public short LeftNodeIndex;
        public short RightNodeIndex;

        [PointerOf("Name")]
        public uint NameAddress;

        [TargetSection("StringsSection")]
        public string Name;
    }
}