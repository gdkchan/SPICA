using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.IO;

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
            RebuildCommands();

            return false;
        }

        internal byte[] GetBytes()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                RebuildCommands();

                foreach (uint Cmd in Commands)
                {
                    Writer.Write(Cmd);
                }

                return MS.ToArray();
            }
        }

        private void RebuildCommands()
        {
            PICACommandWriter Writer = new PICACommandWriter();

            Writer.SetCommand(PICARegister.GPUREG_STENCIL_TEST, Test.ToUInt32(), 13);

            Writer.SetCommand(PICARegister.GPUREG_STENCIL_OP, Operation.ToUInt32());

            Commands = Writer.GetBuffer();
        }
    }
}
