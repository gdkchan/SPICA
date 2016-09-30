using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class TargetSectionAttribute : Attribute
    {
        public string Name;
        public int Order;

        public TargetSectionAttribute(string Name, int Order = 0)
        {
            this.Name = Name;
            this.Order = Order;
        }
    }
}
