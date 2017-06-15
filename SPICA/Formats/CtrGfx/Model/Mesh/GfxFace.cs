using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxFace
    {
        public readonly List<GfxFaceDescriptor> FaceDescriptors;

        private uint[] BufferObjects;

        private uint Flags;

        private uint CommandAllocPtr;

        public GfxFace()
        {
            FaceDescriptors = new List<GfxFaceDescriptor>();
        }
    }
}
