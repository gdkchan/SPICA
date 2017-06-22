using SPICA.Formats.CtrGfx.Texture;
using SPICA.Serialization.Serializer;

using System;

namespace SPICA.Formats.CtrGfx
{
    class GfxComparers
    {
        public static Comparison<RefValue> GetComparisonRaw()
        {
            return new Comparison<RefValue>(CompareBuffer);
        }

        public static int CompareBuffer(RefValue LHS, RefValue RHS)
        {
            if      (LHS.Parent == RHS.Parent)
                return 0;
            else if (LHS.Parent is GfxTexture)
                return -1;
            else
                return 1;
        }
    }
}
