using System;

namespace SPICA.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class IfVersionLAttribute : Attribute
    {
        public int Version;

        public IfVersionLAttribute(int Version)
        {
            this.Version = Version;
        }
    }
}
