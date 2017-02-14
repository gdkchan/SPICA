using SPICA.Formats.CtrH3D;

using System.IO;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.XML
{
    class H3DXml
    {
        public static void Save(string FileName, H3D SceneData)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(H3D));

                Serializer.Serialize(FS, SceneData);
            }
        }
    }
}
