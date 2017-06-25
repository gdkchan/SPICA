using System.Collections.Generic;
using System.Reflection;

namespace SPICA.Serialization.Serializer
{
    class RefValue
    {
        public readonly List<RefValue> Childs;

        public FieldInfo Info;

        public object Parent;
        public object Value;

        public long Position;
        public bool HasLength;
        public bool HasTwoPtr;
        public uint PointerOffset;

        public RefValue()
        {
            Childs = new List<RefValue>();
        }

        public RefValue(object Value) : this()
        {
            this.Value = Value;

            Info = null;

            Parent = null;

            Position      = -1;
            HasLength     = false;
            HasTwoPtr     = false;
            PointerOffset = 0;
        }
    }
}
