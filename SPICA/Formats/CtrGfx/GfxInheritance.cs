namespace SPICA.Formats.CtrGfx
{
    struct GfxInheritance
    {
        private uint Inheritance;

        public int GetDepth()
        {
            int Depth = 0;

            for (int i = 0; i < 32; i++)
            {
                if (((Inheritance >> i) & 1) != 0) Depth++;
            }

            return Depth;
        }

        public int GetClassIndex(int TargetDepth)
        {
            int Depth = 0;
            int Index = 0;

            for (int i = 0; i < 32; i++)
            {
                if (((Inheritance >> i) & 1) != 0)
                {
                    if (Depth++ == TargetDepth) break;

                    Index = 0;
                }
                else
                {
                    Index++;
                }
            }

            if (Depth < TargetDepth) return -1;

            return Index;
        }

        public bool CheckPath(params int[] Indices)
        {
            for (int i = 0; i < Indices.Length; i++)
            {
                if (GetClassIndex(i) != Indices[i]) return false;
            }

            return true;
        }
    }
}
