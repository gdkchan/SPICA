using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents
{
    [Section("MetaDataSection")]
    class H3DMetaData
    {
        [PointerOf("Values")]
        private uint Address;

        [CountOf("Values"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        public H3DTreeNode[] NameTree;
        public H3DMetaDataValue[] Values;

        public H3DMetaDataValue this[int Index]
        {
            get { return Values[Index]; }
            set { Values[Index] = value; }
        }
    }
}
