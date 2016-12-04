using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFMotBoolean
    {
        public string Name;

        public bool[] Values;

        public GFMotBoolean() { }

        public GFMotBoolean(BinaryReader Reader, string Name, int Count)
        {
            this.Name = Name;

            Values = new bool[Count];

            byte Value = 0;

            for (int Index = 0; Index < Count; Index++)
            {
                int Bit = Index & 7;

                if (Bit == 0) Value = Reader.ReadByte();

                Values[Index] = (Value & (1 << Bit)) != 0;
            }
        }
    }
}
