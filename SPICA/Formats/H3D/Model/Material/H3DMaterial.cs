using SPICA.Formats.H3D.Model.Material.Texture;
using SPICA.Formats.H3D.Texture;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

namespace SPICA.Formats.H3D.Model.Material
{
    [Inline]
    public class H3DMaterial : ICustomSerialization, INamed
    {
        public H3DMaterialParams MaterialParams;

        public H3DTexture Texture0;
        public H3DTexture Texture1;
        public H3DTexture Texture2;

        private uint[] TextureCommands;

        [FixedLength(3)]
        public H3DTextureMapper[] TextureMappers;

        public string Texture0Name;
        public string Texture1Name;
        public string Texture2Name;

        public string Name;

        public string ObjectName { get { return Name; } }

        public H3DMaterial()
        {
            TextureMappers = new H3DTextureMapper[3];
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer) { }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
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
