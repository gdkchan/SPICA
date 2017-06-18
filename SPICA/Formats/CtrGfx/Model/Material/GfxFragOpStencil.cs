using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxFragOpStencil : ICustomSerialization
    {
        [Inline, FixedLength(4)] private uint[] Commands;

        [Ignore] public PICAStencilTest Test;

        [Ignore] public PICAStencilOperation Operation;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_STENCIL_TEST: Test = new PICAStencilTest(Param); break;

                    case PICARegister.GPUREG_STENCIL_OP: Operation = new PICAStencilOperation(Param); break;
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
