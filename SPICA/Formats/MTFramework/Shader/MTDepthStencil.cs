using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.MTFramework.Shader
{
    public class MTDepthStencil : MTShaderEffect
    {
        public bool DepthTest;
        public bool DepthWrite;

        public PICATestFunc DepthFunc;
        public PICATestFunc StencilFunc;

        public sbyte StencilRef;

        public MTDepthStencil(BinaryReader Reader)
        {
            byte DepthStencilTest = Reader.ReadByte();

            DepthTest  = (DepthStencilTest & 1) != 0;
            DepthWrite = (DepthStencilTest & 2) != 0;

            DepthFunc   = ((MTTestFunction)((DepthStencilTest >> 2) & 7)).ToPICATestFunc();
            StencilFunc = ((MTTestFunction)((DepthStencilTest >> 5) & 7)).ToPICATestFunc();

            StencilRef = Reader.ReadSByte();

            Reader.ReadUInt16();
            Reader.ReadUInt16();
            Reader.ReadUInt16();
        }
    }
}
