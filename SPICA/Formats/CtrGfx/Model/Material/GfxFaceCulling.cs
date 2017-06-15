using SPICA.PICA.Commands;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public enum GfxFaceCulling : uint
    {
        FrontFace,
        BackFace,
        Always,
        Never
    }

    static class GfxFaceCullingExtensions
    {
        public static PICAFaceCulling ToPICAFaceCulling(this GfxFaceCulling FaceCulling)
        {
            switch (FaceCulling)
            {
                case GfxFaceCulling.FrontFace: return PICAFaceCulling.FrontFace;
                case GfxFaceCulling.BackFace:  return PICAFaceCulling.BackFace;
                case GfxFaceCulling.Always:    return PICAFaceCulling.FrontFace;
                case GfxFaceCulling.Never:     return PICAFaceCulling.Never;
            }

            return 0;
        }
    }
}
