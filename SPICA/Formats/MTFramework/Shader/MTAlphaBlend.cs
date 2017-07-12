using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.MTFramework.Shader
{
    public class MTAlphaBlend : MTShaderEffect
    {
        public PICABlendMode BlendMode;
        public PICABlendFunction BlendFunction;

        public bool RedWrite;
        public bool GreenWrite;
        public bool BlueWrite;
        public bool AlphaWrite;

        public MTAlphaBlend(BinaryReader Reader)
        {
            //First 4 bytes seems to use bit 0 for something else, so we need to rsh the value by 1?
            BlendMode = (PICABlendMode)(Reader.ReadByte() >> 1);

            MTBlendFunction ColorSrcFunc = (MTBlendFunction)(Reader.ReadByte() >> 1);
            MTBlendFunction ColorDstFunc = (MTBlendFunction)(Reader.ReadByte() >> 1);
            PICABlendEquation ColorEqu = (PICABlendEquation)(Reader.ReadByte() >> 1);

            MTBlendFunction AlphaSrcFunc = (MTBlendFunction)Reader.ReadByte();
            MTBlendFunction AlphaDstFunc = (MTBlendFunction)Reader.ReadByte();
            PICABlendEquation AlphaEqu = (PICABlendEquation)Reader.ReadByte();

            byte Padding = Reader.ReadByte(); //?

            byte[] BufferRW =
            {
                Reader.ReadByte(),
                Reader.ReadByte(), //Always 0xf?
                Reader.ReadByte(), //Always 0xf?
                Reader.ReadByte(), //Always 0xf?
                Reader.ReadByte(), //Always 0xf?
                Reader.ReadByte(), //Always 0xf?
                Reader.ReadByte(), //Always 0xf?
                Reader.ReadByte()  //Always 0xf?
            };

            BlendFunction.ColorEquation = ColorEqu;
            BlendFunction.AlphaEquation = AlphaEqu;

            BlendFunction.ColorSrcFunc = ColorSrcFunc.ToPICABlendFunc(false);
            BlendFunction.ColorDstFunc = ColorDstFunc.ToPICABlendFunc(false);

            BlendFunction.AlphaSrcFunc = ColorSrcFunc.ToPICABlendFunc(true);
            BlendFunction.AlphaDstFunc = ColorDstFunc.ToPICABlendFunc(true);

            RedWrite   = (BufferRW[0] & 1) != 0;
            GreenWrite = (BufferRW[0] & 2) != 0;
            BlueWrite  = (BufferRW[0] & 4) != 0;
            AlphaWrite = (BufferRW[0] & 8) != 0;
        }
    }
}
