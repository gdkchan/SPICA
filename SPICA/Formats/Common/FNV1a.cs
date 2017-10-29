using System.Text;

namespace SPICA.Formats.Common
{
    public class FNV1a
    {
        private const uint OffsetBasis = 0x811c9dc5;
        private const uint Prime = 16777619;

        public uint HashCode { get; private set; }

        public FNV1a()
        {
            HashCode = OffsetBasis;
        }

        public FNV1a(uint BaseHash)
        {
            HashCode = BaseHash;
        }

        public void Hash(byte Value)
        {
            HashCode ^= Value;
            HashCode *= Prime;
        }

        public void Hash(ushort Value)
        {
            Hash((byte)(Value >> 0));
            Hash((byte)(Value >> 8));
        }

        public void Hash(uint Value)
        {
            Hash((byte)(Value >>  0));
            Hash((byte)(Value >>  8));
            Hash((byte)(Value >> 16));
            Hash((byte)(Value >> 24));
        }

        public void Hash(sbyte Value)
        {
            Hash((byte)Value);
        }

        public void Hash(short Value)
        {
            Hash((ushort)Value);
        }

        public void Hash(int Value)
        {
            Hash((uint)Value);
        }

        public void Hash(float Value)
        {
            Hash(IOUtils.ToUInt32(Value));
        }

        public void Hash(string Value, bool Unicode = false)
        {
            if (Value != null)
            {
                byte[] Data = Unicode
                    ? Encoding.Unicode.GetBytes(Value)
                    : Encoding.ASCII.GetBytes(Value);

                foreach (byte Character in Data)
                {
                    Hash(Character);
                }
            }
        }

        public void Hash(byte[] Values)
        {
            foreach (byte Value in Values)
            {
                Hash(Value);
            }
        }

        public void Hash(ushort[] Values)
        {
            foreach (ushort Value in Values)
            {
                Hash(Value);
            }
        }

        public void Hash(uint[] Values)
        {
            foreach (uint Value in Values)
            {
                Hash(Value);
            }
        }
    }
}
