using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFMotBoolean
    {
        public string Name;

        public readonly List<bool> Values;

        public GFMotBoolean()
        {
            Values = new List<bool>();
        }

        public GFMotBoolean(BinaryReader Reader, string Name, int Count) : this()
        {
            this.Name = Name;

            byte Value = 0;

            for (int Index = 0; Index < Count; Index++)
            {
                int Bit = Index & 7;

                if (Bit == 0) Value = Reader.ReadByte();

                Values.Add((Value & (1 << Bit)) != 0);
            }
        }
    }
}
