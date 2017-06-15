using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxAlphaTest : ICustomSerialization
    {
        [Inline, FixedLength(2)] private uint[] Commands;

        [Ignore] public PICAAlphaTest Test;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                if (Cmd.Register == PICARegister.GPUREG_FRAGOP_ALPHA_TEST)
                {
                    Test = new PICAAlphaTest(Cmd.Parameters[0]);
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
