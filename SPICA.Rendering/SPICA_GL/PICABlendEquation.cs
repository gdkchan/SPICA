using OpenTK.Graphics.OpenGL;

using SPICA.PICA.Commands;

using System;

namespace SPICA.Rendering.SPICA_GL
{
    static class PICABlendEquationExtensions
    {
        public static BlendEquationMode ToBlendEquation(this PICABlendEquation Equation)
        {
            switch (Equation)
            {
                case PICABlendEquation.FuncAdd:             return BlendEquationMode.FuncAdd;
                case PICABlendEquation.FuncSubtract:        return BlendEquationMode.FuncSubtract;
                case PICABlendEquation.FuncReverseSubtract: return BlendEquationMode.FuncReverseSubtract;
                case PICABlendEquation.Min:                 return BlendEquationMode.Min;
                case PICABlendEquation.Max:                 return BlendEquationMode.Max;

                default: throw new ArgumentException("Invalid Blend equation!");
            }
        }
    }
}
