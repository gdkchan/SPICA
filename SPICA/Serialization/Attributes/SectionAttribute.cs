using System;

namespace SPICA.Serialization.Attributes
{
    enum SectionName
    {
        Contents,
        Strings,
        Commands,
        RawDataTex,
        RawDataVtx,
        RawExtTex,
        RawExtVtx
    }

    [AttributeUsage(AttributeTargets.Field)]
    class SectionAttribute : Attribute
    {
        public SectionName Name;

        public SectionAttribute(SectionName Name)
        {
            this.Name = Name;
        }
    }
}
