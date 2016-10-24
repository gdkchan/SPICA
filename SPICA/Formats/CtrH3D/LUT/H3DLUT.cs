using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.LUT
{
    public class H3DLUT : INamed
    {
        public List<H3DLUTSampler> Samplers;

        public string Name;

        public string ObjectName { get { return Name; } }

        public H3DLUT()
        {
            Samplers = new List<H3DLUTSampler>();
        }
    }
}
