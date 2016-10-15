using System.Collections.Generic;

namespace SPICA.Formats.H3D.LUT
{
    class H3DLUT : INamed
    {
        public List<H3DLUTSampler> Samplers;

        public string Name;

        public string ObjectName { get { return Name; } }
    }
}
