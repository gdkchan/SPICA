using System;

namespace SPICA.Serialization.Attributes
{
    class PaddingAttribute : Attribute
    {
        public int Size;

        public PaddingAttribute(int Size)
        {
            this.Size = Size;
        }
    }
}
