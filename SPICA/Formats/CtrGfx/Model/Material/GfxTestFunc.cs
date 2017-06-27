using SPICA.PICA.Commands;

using System;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    enum GfxTestFunc
    {
        Never,
        Always,
        Less,
        Lequal,
        Equal,
        Gequal,
        Greater,
        Notequal
    }

    static class GfxTestFuncExtensions
    {
        public static GfxTestFunc ToGfxTestFunc(this PICATestFunc TestFunc)
        {
            //Too much to ask to use the same values the GPU use?
            //Really hate to convert between those stupid inconsistent enumerations.
            switch (TestFunc)
            {
                case PICATestFunc.Never:    return GfxTestFunc.Never;
                case PICATestFunc.Always:   return GfxTestFunc.Always;
                case PICATestFunc.Less:     return GfxTestFunc.Less;
                case PICATestFunc.Lequal:   return GfxTestFunc.Lequal;
                case PICATestFunc.Equal:    return GfxTestFunc.Equal;
                case PICATestFunc.Gequal:   return GfxTestFunc.Gequal;
                case PICATestFunc.Greater:  return GfxTestFunc.Greater;
                case PICATestFunc.Notequal: return GfxTestFunc.Notequal;

                default: throw new ArgumentException($"Invalid PICA Test Function {TestFunc}!");
            }
        }
    }
}
