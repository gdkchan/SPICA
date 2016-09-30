using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field, AllowMultiple = true)]
    class SectionAttribute : Attribute
    {
        public string Name;
        public uint Align;

        public SectionAttribute(string Name, uint Align = 1)
        {
            this.Name = Name;
            this.Align = Align;
        }
    }
}