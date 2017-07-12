using OpenTK.Graphics.OpenGL;

using SPICA.PICA.Commands;

using System;

namespace SPICA.Rendering.SPICA_GL
{
    static class PICABlendFuncExtensions
    {
        public static All ToBlendingFactor(this PICABlendFunc Func)
        {
            switch (Func)
            {
                case PICABlendFunc.Zero:                     return All.Zero;
                case PICABlendFunc.One:                      return All.One;
                case PICABlendFunc.SourceColor:              return All.SrcColor;
                case PICABlendFunc.OneMinusSourceColor:      return All.OneMinusSrcColor;
                case PICABlendFunc.DestinationColor:         return All.DstColor;
                case PICABlendFunc.OneMinusDestinationColor: return All.OneMinusDstColor;
                case PICABlendFunc.SourceAlpha:              return All.SrcAlpha;
                case PICABlendFunc.OneMinusSourceAlpha:      return All.OneMinusSrcAlpha;
                case PICABlendFunc.DestinationAlpha:         return All.DstAlpha;
                case PICABlendFunc.OneMinusDestinationAlpha: return All.OneMinusDstAlpha;
                case PICABlendFunc.ConstantColor:            return All.ConstantColor;
                case PICABlendFunc.OneMinusConstantColor:    return All.OneMinusConstantColor;
                case PICABlendFunc.ConstantAlpha:            return All.ConstantAlpha;
                case PICABlendFunc.OneMinusConstantAlpha:    return All.OneMinusConstantAlpha;
                case PICABlendFunc.SourceAlphaSaturate:      return All.SrcAlphaSaturate;

                default: throw new ArgumentException("Invalid Blend function!");
            }
        }

        public static BlendingFactorSrc ToBlendingFactorSrc(this PICABlendFunc Func)
        {
            return (BlendingFactorSrc)ToBlendingFactor(Func);
        }

        public static BlendingFactorDest ToBlendingFactorDest(this PICABlendFunc Func)
        {
            return (BlendingFactorDest)ToBlendingFactor(Func);
        }

        public static void SetGL(this PICABlendFunction BlendFunction)
        {
            GL.BlendEquationSeparate(
                BlendFunction.ColorEquation.ToBlendEquation(),
                BlendFunction.AlphaEquation.ToBlendEquation());

            GL.BlendFuncSeparate(
                BlendFunction.ColorSrcFunc.ToBlendingFactorSrc(),
                BlendFunction.ColorDstFunc.ToBlendingFactorDest(),
                BlendFunction.AlphaSrcFunc.ToBlendingFactorSrc(),
                BlendFunction.AlphaDstFunc.ToBlendingFactorDest());
        }
    }
}
