using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class TargetSectionAttribute : Attribute
    {
        public string Name;
        public int Prio;

        public TargetSectionAttribute(string Name, int Prio = 0)
        {
            this.Name = Name;
            this.Prio = Prio;
        }
    }
}
