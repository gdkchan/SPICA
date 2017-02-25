using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.Formats.CtrH3D.LUT
{
    public class H3DLUT : INamed
    {
        public List<H3DLUTSampler> Samplers;

        private string _Name;

        [XmlAttribute]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public H3DLUT()
        {
            Samplers = new List<H3DLUTSampler>();
        }
    }
}
