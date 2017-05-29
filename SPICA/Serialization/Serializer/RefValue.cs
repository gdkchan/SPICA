using System.Reflection;

namespace SPICA.Serialization.Serializer
{
    struct RefValue
    {
        public FieldInfo Info;
        public object Parent;
        public object Value;
        public long Position;
        public bool HasLength;
        public bool HasTwoPtr;
        public uint PointerOffset;
    }
}
