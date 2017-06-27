using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.IO;

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
            PICACommandWriter Writer = new PICACommandWriter();

            Writer.SetCommand(PICARegister.GPUREG_FRAGOP_ALPHA_TEST, Test.ToUInt32());

            Commands = Writer.GetBuffer();

            return false;
        }

        internal byte[] GetBytes()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write((byte)(Test.Enabled ? 1 : 0));
                Writer.Write((uint)Test.Function.ToGfxTestFunc());
                Writer.Write(Test.Reference / (float)0xff);

                return MS.ToArray();
            }
        }
    }
}
