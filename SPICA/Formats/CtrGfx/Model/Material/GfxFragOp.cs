using SPICA.Math3D;
using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxFragOp
    {
        public GfxFragOpDepth   Depth;
        public GfxFragOpBlend   Blend;
        public GfxFragOpStencil Stencil;

        internal byte[] GetBytes()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write((uint)Depth.Flags);
                Writer.Write((uint)Depth.ColorMask.DepthFunc.ToGfxTestFunc());
                Writer.Write(0u);
                Writer.Write((uint)Blend.Mode);
                Writer.Write(Blend.Color.ToVector4());

                if (Blend.Mode == GfxFragOpBlendMode.LogicalOp)
                {
                    Writer.Write((uint)Blend.LogicalOperation.ToGfxLogicalOp());
                }
                else
                {
                    Writer.Write((uint)GfxLogicalOp.Copy);
                }

                switch (Blend.Mode)
                {
                    case GfxFragOpBlendMode.Blend:
                        Writer.Write(GetGLBlend(Blend.Function.ColorSrcFunc));
                        Writer.Write(GetGLBlend(Blend.Function.ColorDstFunc));
                        Writer.Write(GetGLBlend(Blend.Function.ColorEquation));
                        Writer.Write(GetGLBlend(Blend.Function.ColorSrcFunc));
                        Writer.Write(GetGLBlend(Blend.Function.ColorDstFunc));
                        Writer.Write(GetGLBlend(Blend.Function.ColorEquation));
                        break;

                    case GfxFragOpBlendMode.BlendSeparate:
                        Writer.Write(GetGLBlend(Blend.Function.ColorSrcFunc));
                        Writer.Write(GetGLBlend(Blend.Function.ColorDstFunc));
                        Writer.Write(GetGLBlend(Blend.Function.ColorEquation));
                        Writer.Write(GetGLBlend(Blend.Function.AlphaSrcFunc));
                        Writer.Write(GetGLBlend(Blend.Function.AlphaDstFunc));
                        Writer.Write(GetGLBlend(Blend.Function.AlphaEquation));
                        break;

                    default:
                        Writer.Write(GetGLBlend(PICABlendFunc.SourceAlpha));
                        Writer.Write(GetGLBlend(PICABlendFunc.OneMinusSourceAlpha));
                        Writer.Write(GetGLBlend(PICABlendEquation.FuncAdd));
                        Writer.Write(GetGLBlend(PICABlendFunc.One));
                        Writer.Write(GetGLBlend(PICABlendFunc.Zero));
                        Writer.Write(GetGLBlend(PICABlendEquation.FuncAdd));
                        break;
                }

                Writer.Write(Stencil.GetBytes());

                return MS.ToArray();
            }
        }

        private uint GetGLBlend(PICABlendFunc BlendFunc)
        {
            switch (BlendFunc)
            {
                case PICABlendFunc.Zero:                     return 0x0000;
                case PICABlendFunc.One:                      return 0x0001;
                case PICABlendFunc.SourceColor:              return 0x0300;
                case PICABlendFunc.OneMinusSourceColor:      return 0x0301;
                case PICABlendFunc.DestinationColor:         return 0x0306;
                case PICABlendFunc.OneMinusDestinationColor: return 0x0307;
                case PICABlendFunc.SourceAlpha:              return 0x0302;
                case PICABlendFunc.OneMinusSourceAlpha:      return 0x0303;
                case PICABlendFunc.DestinationAlpha:         return 0x0304;
                case PICABlendFunc.OneMinusDestinationAlpha: return 0x0305;
                case PICABlendFunc.ConstantColor:            return 0x8001;
                case PICABlendFunc.OneMinusConstantColor:    return 0x8002;
                case PICABlendFunc.ConstantAlpha:            return 0x8003;
                case PICABlendFunc.OneMinusConstantAlpha:    return 0x8004;
                case PICABlendFunc.SourceAlphaSaturate:      return 0x0308;
            }

            return 0;
        }

        private uint GetGLBlend(PICABlendEquation BlendEquation)
        {
            switch (BlendEquation)
            {
                case PICABlendEquation.FuncAdd:             return 0x8006;
                case PICABlendEquation.FuncSubtract:        return 0x800a;
                case PICABlendEquation.FuncReverseSubtract: return 0x800b;
                case PICABlendEquation.Min:                 return 0x8007;
                case PICABlendEquation.Max:                 return 0x8008;
            }

            return 0;
        }
    }
}
