using System;

namespace SPICA.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class FixedLengthAttribute : Attribute
    {
        public int Length;

        public FixedLengthAttribute(int Length)
        {
            this.Length = Length;
        }
    }
}
