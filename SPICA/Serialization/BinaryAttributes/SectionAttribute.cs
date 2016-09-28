using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field, AllowMultiple = true)]
    class SectionAttribute : Attribute
    {
        public string Name;

        public SectionAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}