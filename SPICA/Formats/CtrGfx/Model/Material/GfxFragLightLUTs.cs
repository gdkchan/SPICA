using System.IO;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public class GfxFragLightLUTs
    {
        public GfxFragLightLUT ReflecR;
        public GfxFragLightLUT ReflecG;
        public GfxFragLightLUT ReflecB;
        public GfxFragLightLUT Dist0;
        public GfxFragLightLUT Dist1;
        public GfxFragLightLUT Fresnel;

        internal byte[] GetBytes()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                if (ReflecR != null)
                {
                    Writer.Write((uint)ReflecR.Input);
                    Writer.Write((uint)ReflecR.Scale);
                }

                if (ReflecG != null)
                {
                    Writer.Write((uint)ReflecG.Input);
                    Writer.Write((uint)ReflecG.Scale);
                }

                if (ReflecB != null)
                {
                    Writer.Write((uint)ReflecB.Input);
                    Writer.Write((uint)ReflecB.Scale);
                }

                if (Dist0 != null)
                {
                    Writer.Write((uint)Dist0.Input);
                    Writer.Write((uint)Dist0.Scale);
                }

                if (Dist1 != null)
                {
                    Writer.Write((uint)Dist1.Input);
                    Writer.Write((uint)Dist1.Scale);
                }

                if (Fresnel != null)
                {
                    Writer.Write((uint)Fresnel.Input);
                    Writer.Write((uint)Fresnel.Scale);
                }

                return MS.ToArray();
            }
        }
    }
}
