using SPICA.PICA.Commands;

using System;

namespace SPICA.Formats.MTFramework.Shader
{
    public enum MTTestFunction
    {
        Never,
        Less,
        Equal,
        Lequal,
        Greater,
        Notequal,
        Gequal,
        Always
    }

    static class MTTestFunctionExtensions
    {
        public static PICATestFunc ToPICATestFunc(this MTTestFunction TestFunc)
        {
            switch (TestFunc)
            {
                case MTTestFunction.Never:    return PICATestFunc.Never;
                case MTTestFunction.Less:     return PICATestFunc.Less;
                case MTTestFunction.Equal:    return PICATestFunc.Equal;
                case MTTestFunction.Lequal:   return PICATestFunc.Lequal;
                case MTTestFunction.Greater:  return PICATestFunc.Greater;
                case MTTestFunction.Notequal: return PICATestFunc.Notequal;
                case MTTestFunction.Gequal:   return PICATestFunc.Gequal;
                case MTTestFunction.Always:   return PICATestFunc.Always;

                default: throw new ArgumentException("Invalid Test Function value!");
            }
        }
    }
}
