using System.Numerics;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEVector3
    {
        [XmlAttribute] public string sid;

        [XmlText] public string data;

        public static DAEVector3 Empty
        {
            get => new DAEVector3() { data = "0 0 0" };
        }

        public void Set(Vector3 Vector)
        {
            data = DAEUtils.VectorStr(Vector);
        }
    }
}
