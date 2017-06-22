using System.IO;

namespace SPICA.Serialization
{
    class BitWriter
    {
        private BinaryWriter Writer;

        private uint Bools;
        private int  Index;

        public BitWriter(BinaryWriter Writer)
        {
            this.Writer = Writer;
        }

        public void WriteBit(bool Value)
        {
            Bools |= ((Value ? 1u : 0u) << Index);

            if (++Index == 32)
            {
                Writer.Write(Bools);

                Index = 0;
                Bools = 0;
            }
        }

        public void Flush()
        {
            if (Index != 0)
            {
                Writer.Write(Bools);
            }
        }
    }
}
