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

        public RefValue(object Value)
        {
            this.Value = Value;

            Info          = null;
            Parent        = null;
            Position      = -1;
            HasLength     = false;
            HasTwoPtr     = false;
            PointerOffset = 0;
        }
    }
}
