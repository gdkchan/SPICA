using System.IO;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxFragOp
    {
        public GfxFragOpDepth   Depth;
        public GfxFragOpBlend   Blend;
        public GfxFragOpStencil Stencil;

        internal byte[] GetBytes()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write(Depth.GetHashCode());
                Writer.Write(Blend.GetHashCode());
                Writer.Write(Stencil.GetHashCode());

                return MS.ToArray();
            }
        }
    }
}
