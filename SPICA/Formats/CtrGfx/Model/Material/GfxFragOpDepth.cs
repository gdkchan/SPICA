using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxFragOpDepth : ICustomSerialization
    {
        public GfxFragOpDepthFlags Flags;

        [Inline, FixedLength(4)] private uint[] Commands;

        [Ignore] public PICADepthColorMask ColorMask;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                if (Cmd.Register == PICARegister.GPUREG_DEPTH_COLOR_MASK)
                {
                    ColorMask = new PICADepthColorMask(Cmd.Parameters[0]);
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            PICACommandWriter Writer = new PICACommandWriter();

            uint ZDepth;

            if ((uint)ColorMask.DepthFunc > 1)
                ZDepth = (uint)ColorMask.DepthFunc > 5 ? 2u : 3u;
            else
                ZDepth = (uint)ColorMask.DepthFunc;

            Writer.SetCommand(PICARegister.GPUREG_DEPTH_COLOR_MASK, ColorMask.ToUInt32(), 1);

            Writer.SetCommand(PICARegister.GPUREG_GAS_DELTAZ_DEPTH, ZDepth << 24, 8);

            Commands = Writer.GetBuffer();

            return false;
        }
    }
}
