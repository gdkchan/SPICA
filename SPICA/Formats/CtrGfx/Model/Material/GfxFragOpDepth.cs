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
            //TODO

            return false;
        }
    }
}
