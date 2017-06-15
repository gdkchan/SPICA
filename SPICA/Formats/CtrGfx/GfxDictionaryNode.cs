using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx
{
    class GfxDictionaryNode<T> : IPatriciaTreeNode
    {
        private uint   _ReferenceBit;
        private ushort _LeftNodeIndex;
        private ushort _RightNodeIndex;
        private string _Name;

        public T Value;

        public uint ReferenceBit
        {
            get
            {
                return _ReferenceBit;
            }
            set
            {
                _ReferenceBit = value;
            }
        }

        public ushort LeftNodeIndex
        {
            get
            {
                return _LeftNodeIndex;
            }
            set
            {
                _LeftNodeIndex = value;
            }
        }

        public ushort RightNodeIndex
        {
            get
            {
                return _RightNodeIndex;
            }
            set
            {
                _RightNodeIndex = value;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
    }
}
