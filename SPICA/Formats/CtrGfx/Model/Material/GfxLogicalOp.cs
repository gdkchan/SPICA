using SPICA.PICA.Commands;

using System;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    enum GfxLogicalOp
    {
        Clear,
        Copy,
        Noop,
        Set,
        CopyInverted,
        Invert,
        AndReverse,
        OrReverse,
        And,
        Or,
        Nand,
        Nor,
        Xor,
        Equiv,
        AndInverted,
        OrInverted
    }

    static class GfxLogicalOperationExtensions
    {
        public static GfxLogicalOp ToGfxLogicalOp(this PICALogicalOp LogicalOp)
        {
            switch (LogicalOp)
            {
                case PICALogicalOp.Clear:        return GfxLogicalOp.Clear;
                case PICALogicalOp.Copy:         return GfxLogicalOp.Copy;
                case PICALogicalOp.Noop:         return GfxLogicalOp.Noop;
                case PICALogicalOp.Set:          return GfxLogicalOp.Set;
                case PICALogicalOp.CopyInverted: return GfxLogicalOp.CopyInverted;
                case PICALogicalOp.Invert:       return GfxLogicalOp.Invert;
                case PICALogicalOp.AndReverse:   return GfxLogicalOp.AndReverse;
                case PICALogicalOp.OrReverse:    return GfxLogicalOp.OrReverse;
                case PICALogicalOp.And:          return GfxLogicalOp.And;
                case PICALogicalOp.Or:           return GfxLogicalOp.Or;
                case PICALogicalOp.Nand:         return GfxLogicalOp.Nand;
                case PICALogicalOp.Nor:          return GfxLogicalOp.Nor;
                case PICALogicalOp.Xor:          return GfxLogicalOp.Xor;
                case PICALogicalOp.Equiv:        return GfxLogicalOp.Equiv;
                case PICALogicalOp.AndInverted:  return GfxLogicalOp.AndInverted;
                case PICALogicalOp.OrInverted:   return GfxLogicalOp.OrInverted;

                default: throw new ArgumentException($"Invalid Logical Operation {LogicalOp}!");
            }
        }
    }
}
