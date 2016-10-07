using SPICA.Serialization.Attributes;

using System;
using System.Text;

namespace SPICA.Math3D
{
    [Inline]
    class Matrix3x4
    {
        [FixedLength(4 * 3), Inline]
        public float[] Elems;

        public Matrix3x4()
        {
            Elems = new float[4 * 3];
        }

        public float this[int Row, int Col]
        {
            get { return Elems[(Row * 4) + Col]; }
            set { Elems[(Row * 4) + Col] = value; }
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();

            for (int Row = 0; Row < 3; Row++)
            {
                for (int Col = 0; Col < 4; Col++)
                {
                    SB.Append(string.Format("M{0}{1}: {2,-16}", Row + 1, Col + 1, this[Row, Col]));
                }

                SB.Append(Environment.NewLine);
            }

            return SB.ToString();
        }
    }
}
