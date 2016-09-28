using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class SectionPointerOfAttribute : Attribute
    {
        public string Name;

        public SectionPointerOfAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}
