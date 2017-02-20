using SPICA.Formats.CtrH3D;

using System.IO;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.XML
{
    class H3DXML
    {
        private H3D SceneData;

        public H3DXML(H3D SceneData)
        {
            this.SceneData = SceneData;
        }

        public void Save(string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(H3D));

                Serializer.Serialize(FS, SceneData);
            }
        }
    }
}
