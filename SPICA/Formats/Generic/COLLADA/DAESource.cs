using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAESource
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        [XmlElement(IsNullable = false)] public DAEArray Name_array;
        [XmlElement(IsNullable = false)] public DAEArray float_array;

        public DAESourceTechniqueCommon technique_common = new DAESourceTechniqueCommon();

        public DAESource() { }

        public DAESource(string Name, int Stride, string[] Elements, params string[] Accessors)
        {
            name = Name;
            id = $"{Name}_id";

            DAEArray Array = new DAEArray()
            {
                id    = $"{Name}_array_id",
                count = (uint)(Elements.Length * Stride),
                data  = string.Join(" ", Elements)
            };

            if (Accessors[1] == "Name")
                Name_array = Array;
            else
                float_array = Array;

            technique_common.accessor.source = $"#{Array.id}";
            technique_common.accessor.count  = (uint)Elements.Length;
            technique_common.accessor.stride = (uint)Stride;

            for (int Index = 0; Index < Accessors.Length; Index += 2)
            {
                technique_common.accessor.AddParam(Accessors[Index + 0], Accessors[Index + 1]);
            }
        }
    }

    public class DAESourceTechniqueCommon
    {
        public DAEAccessor accessor = new DAEAccessor();
    }
}
