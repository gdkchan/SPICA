using SPICA.Formats.Common;

using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.LUT
{
    public class H3DLUT : INamed
    {
        public readonly List<H3DLUTSampler> Samplers;

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public H3DLUT()
        {
            Samplers = new List<H3DLUTSampler>();
        }
    }
}
