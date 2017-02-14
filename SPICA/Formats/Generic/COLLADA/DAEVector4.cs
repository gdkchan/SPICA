using SPICA.Math3D;

using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEVector4
    {
        [XmlAttribute] public string sid;

        [XmlText] public string data;

        public static DAEVector4 Empty
        {
            get
            {
                return new DAEVector4 { data = "0 0 0 0" };
            }
        }

        public void Set(Vector4D Vector)
        {
            data = Vector.ToSerializableString();
        }
    }
}
