using SPICA.Serialization.BinaryAttributes;

using System;
using System.Text;

namespace SPICA.Math
{
    class Matrix3x3
    {
        [FixedCount(3 * 3)]
        public float[] Elems;

        public Matrix3x3()
        {
            Elems = new float[3 * 3];
        }

        public float this[int Row, int Col]
        {
            get { return Elems[(Row * 3) + Col]; }
            set { Elems[(Row * 3) + Col] = value; }
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();

            for (int Row = 0; Row < 3; Row++)
            {
                for (int Col = 0; Col < 3; Col++)
                {
                    SB.Append(string.Format("M{0}{1}: {2,-16}", Row + 1, Col + 1, this[Row, Col]));
                }

                SB.Append(Environment.NewLine);
            }

            return SB.ToString().TrimEnd();
        }
    }
}
