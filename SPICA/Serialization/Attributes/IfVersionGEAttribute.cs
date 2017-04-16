using System;

namespace SPICA.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class IfVersionGEAttribute : Attribute
    {
        public int Version;

        public IfVersionGEAttribute(int Version)
        {
            this.Version = Version;
        }
    }
}
