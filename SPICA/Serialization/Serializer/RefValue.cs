using System.Reflection;

namespace SPICA.Serialization.Serializer
{
    struct RefValue
    {
        public delegate void OnSerialize(BinarySerializer Serializer, object Value);

        public OnSerialize Serialize;
        public FieldInfo Info;
        public object Value;
        public long Position;
        public bool HasLength;
        public bool HasTwoPtr;
    }
}
