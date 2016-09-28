using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class SectionLengthOfAttribute : Attribute
    {
        public string Name;

        public SectionLengthOfAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}
