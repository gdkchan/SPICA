using SPICA.Serialization.Attributes;

using System;
using System.Text;
using System.Xml.Serialization;

namespace SPICA.Math3D
{
    [Inline]
    public class Matrix3x3
    {
        [FixedLength(3 * 3), Inline]
        private float[] Elems;

        public Matrix3x3()
        {
            Elems = new float[3 * 3];

            this[0, 0] = 1;
            this[1, 1] = 1;
            this[2, 2] = 1;
        }

        [XmlAttribute] public float M11 { get { return this[0, 0]; } set { this[0, 0] = value; } }
        [XmlAttribute] public float M12 { get { return this[0, 1]; } set { this[0, 1] = value; } }
        [XmlAttribute] public float M13 { get { return this[0, 2]; } set { this[0, 2] = value; } }

        [XmlAttribute] public float M21 { get { return this[1, 0]; } set { this[1, 0] = value; } }
        [XmlAttribute] public float M22 { get { return this[1, 1]; } set { this[1, 1] = value; } }
        [XmlAttribute] public float M23 { get { return this[1, 2]; } set { this[1, 2] = value; } }

        [XmlAttribute] public float M31 { get { return this[2, 0]; } set { this[2, 0] = value; } }
        [XmlAttribute] public float M32 { get { return this[2, 1]; } set { this[2, 1] = value; } }
        [XmlAttribute] public float M33 { get { return this[2, 2]; } set { this[2, 2] = value; } }

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

            return SB.ToString();
        }
    }
}
