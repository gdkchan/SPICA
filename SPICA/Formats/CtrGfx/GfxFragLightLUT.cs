using SPICA.PICA.Commands;

namespace SPICA.Formats.CtrGfx
{
    public class GfxFragLightLUT
    {
        public PICALUTInput Input;
        public PICALUTScale Scale;

        public readonly GfxLUTReference Sampler;

        public GfxFragLightLUT()
        {
            Sampler = new GfxLUTReference();
        }
    }
}
