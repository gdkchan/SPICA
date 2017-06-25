using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx
{
    struct GfxDictionaryNode<T> : IPatriciaTreeNode
    {
        private uint   _ReferenceBit;
        private ushort _LeftNodeIndex;
        private ushort _RightNodeIndex;
        private string _Name;

        public T Value;

        public uint ReferenceBit
        {
            get => _ReferenceBit;
            set => _ReferenceBit = value;
        }

        public ushort LeftNodeIndex
        {
            get => _LeftNodeIndex;
            set => _LeftNodeIndex = value;
        }

        public ushort RightNodeIndex
        {
            get => _RightNodeIndex;
            set => _RightNodeIndex = value;
        }

        public string Name
        {
            get => _Name;
            set => _Name = value;
        }
    }
}
