using System;

namespace SPICA.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class CustomLengthAttribute : Attribute
    {
        public LengthPos  Pos;
        public LengthSize Size;

        public CustomLengthAttribute(LengthPos Pos, LengthSize Size)
        {
            this.Pos  = Pos;
            this.Size = Size;
        }
    }
}
