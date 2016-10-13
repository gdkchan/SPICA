using SPICA.Formats.H3D.Model.Material.Texture;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

namespace SPICA.Formats.H3D.Model.Material
{
    class H3DMaterial : ICustomSerialization
    {
        public H3DMaterialParams MaterialParams;

        public uint Texture0ConfigAddress;
        public uint Texture1ConfigAddress;
        public uint Texture2ConfigAddress;
        public uint[] TextureCommands;

        [FixedLength(3)]
        public H3DTextureMapper[] TextureMappers;

        public string Texture0Name;
        public string Texture1Name;
        public string Texture2Name;
        public string Name;

        public void Deserialize(BinaryDeserializer Deserializer) { }

        public bool Serialize(BinarySerializer Serializer)
        {
            //The original tool seems to add those (usually unused) names with the silhouette suffix
            Serializer.Strings.Values.Add(new RefValue
            {
                Position = -1,
                Value = Name + "-silhouette"
            });

            return false;
        }
    }
}
