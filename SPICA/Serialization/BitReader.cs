using System.IO;

namespace SPICA.Serialization
{
    class BitReader
    {
        private BinaryReader Reader;

        private uint Bools;
        private int  Index;

        public BitReader(BinaryReader Reader)
        {
            this.Reader = Reader;
        }

        public bool ReadBit()
        {
            if ((Index++ & 0x1f) == 0)
            {
                Bools = Reader.ReadUInt32();
            }

            bool Value = (Bools & 1) != 0;

            Bools >>= 1;

            return Value;
        }
    }
}
