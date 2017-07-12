using SPICA.PICA.Commands;

using System;

namespace SPICA.Formats.MTFramework.Shader
{
    public enum MTBlendFunction
    {
        Zero,
        One,
        SourceColor,
        InvSourceColor,
        SourceAlpha,
        InvSourceAlpha,
        DestAlpha,
        InvDestAlpha,
        DestColor,
        InvDestColor,
        SourceAlphaSaturate,
        BlendFactor,
        InvBlendFactor
    }

    static class MTBlendFuncExtensions
    {
        public static PICABlendFunc ToPICABlendFunc(this MTBlendFunction BlendFunc, bool Alpha)
        {
            switch (BlendFunc)
            {
                case MTBlendFunction.Zero:                return PICABlendFunc.Zero;
                case MTBlendFunction.One:                 return PICABlendFunc.One;
                case MTBlendFunction.SourceColor:         return PICABlendFunc.SourceColor;
                case MTBlendFunction.InvSourceColor:      return PICABlendFunc.OneMinusSourceColor;
                case MTBlendFunction.SourceAlpha:         return PICABlendFunc.SourceAlpha;
                case MTBlendFunction.InvSourceAlpha:      return PICABlendFunc.OneMinusSourceAlpha;
                case MTBlendFunction.DestAlpha:           return PICABlendFunc.DestinationAlpha;
                case MTBlendFunction.InvDestAlpha:        return PICABlendFunc.OneMinusDestinationAlpha;
                case MTBlendFunction.DestColor:           return PICABlendFunc.DestinationColor;
                case MTBlendFunction.InvDestColor:        return PICABlendFunc.OneMinusDestinationColor;
                case MTBlendFunction.SourceAlphaSaturate: return PICABlendFunc.SourceAlphaSaturate;
                case MTBlendFunction.BlendFactor:
                    return Alpha
                        ? PICABlendFunc.ConstantAlpha
                        : PICABlendFunc.ConstantColor;
                case MTBlendFunction.InvBlendFactor:
                    return Alpha
                        ? PICABlendFunc.OneMinusConstantAlpha
                        : PICABlendFunc.OneMinusConstantColor;

                default: throw new ArgumentException("Invalid Blending Function value!");
            }
        }
    }
}
