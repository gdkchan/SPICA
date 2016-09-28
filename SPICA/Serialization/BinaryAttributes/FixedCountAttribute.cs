using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class FixedCountAttribute : Attribute
    {
        public int Count;

        public FixedCountAttribute(int Count)
        {
            this.Count = Count;
        }
    }
}
