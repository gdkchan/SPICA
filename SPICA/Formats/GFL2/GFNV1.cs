using System.Text;

namespace SPICA.Formats.GFL2
{
    class GFNV1
    {
        private const uint Prime = 16777619;

        public uint HashCode { get; private set; }

        public GFNV1()
        {
            HashCode = Prime;
        }

        public void Hash(byte Value)
        {
            HashCode *= Prime;
            HashCode ^= Value;
        }

        public void Hash(string Value)
        {
            if (Value != null)
            {
                byte[] Data = Encoding.ASCII.GetBytes(Value);

                foreach (byte Character in Data)
                {
                    Hash(Character);
                }
            }
        }
    }
}
