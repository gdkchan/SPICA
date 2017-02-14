using SPICA.Serialization.Attributes;

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SPICA.Math3D
{
    [Inline]
    public class Matrix4x4
    {
        [FixedLength(4 * 4), Inline]
        private float[] Elems;

        public Matrix4x4()
        {
            Elems = new float[4 * 4];

            this[0, 0] = 1;
            this[1, 1] = 1;
            this[2, 2] = 1;
            this[3, 3] = 1;
        }

        public Matrix4x4(BinaryReader Reader)
        {
            Elems = new float[4 * 4];

            for (int Row = 0; Row < 4; Row++)
            {
                for (int Col = 0; Col < 4; Col++)
                {
                    this[Row, Col] = Reader.ReadSingle();
                }
            }
        }

        public static Matrix4x4 Empty
        {
            get
            {
                return new Matrix4x4
                {
                    M11 = 0,
                    M22 = 0,
                    M33 = 0,
                    M44 = 0
                };
            }
        }

        [XmlAttribute] public float M11 { get { return this[0, 0]; } set { this[0, 0] = value; } }
        [XmlAttribute] public float M12 { get { return this[0, 1]; } set { this[0, 1] = value; } }
        [XmlAttribute] public float M13 { get { return this[0, 2]; } set { this[0, 2] = value; } }
        [XmlAttribute] public float M14 { get { return this[0, 3]; } set { this[0, 3] = value; } }

        [XmlAttribute] public float M21 { get { return this[1, 0]; } set { this[1, 0] = value; } }
        [XmlAttribute] public float M22 { get { return this[1, 1]; } set { this[1, 1] = value; } }
        [XmlAttribute] public float M23 { get { return this[1, 2]; } set { this[1, 2] = value; } }
        [XmlAttribute] public float M24 { get { return this[1, 3]; } set { this[1, 3] = value; } }

        [XmlAttribute] public float M31 { get { return this[2, 0]; } set { this[2, 0] = value; } }
        [XmlAttribute] public float M32 { get { return this[2, 1]; } set { this[2, 1] = value; } }
        [XmlAttribute] public float M33 { get { return this[2, 2]; } set { this[2, 2] = value; } }
        [XmlAttribute] public float M34 { get { return this[2, 3]; } set { this[2, 3] = value; } }

        [XmlAttribute] public float M41 { get { return this[3, 0]; } set { this[3, 0] = value; } }
        [XmlAttribute] public float M42 { get { return this[3, 1]; } set { this[3, 1] = value; } }
        [XmlAttribute] public float M43 { get { return this[3, 2]; } set { this[3, 2] = value; } }
        [XmlAttribute] public float M44 { get { return this[3, 3]; } set { this[3, 3] = value; } }

        public float this[int Row, int Col]
        {
            get { return Elems[(Row * 4) + Col]; }
            set { Elems[(Row * 4) + Col] = value; }
        }

        public static Matrix3x4 RotateX(float Angle)
        {
            return new Matrix3x4
            {
                M22 = (float)Math.Cos(Angle),
                M23 = -(float)Math.Sin(Angle),
                M32 = (float)Math.Sin(Angle),
                M33 = (float)Math.Cos(Angle)
            };
        }

        public static Matrix3x4 RotateY(float Angle)
        {
            return new Matrix3x4
            {
                M11 = (float)Math.Cos(Angle),
                M13 = (float)Math.Sin(Angle),
                M31 = -(float)Math.Sin(Angle),
                M33 = (float)Math.Cos(Angle)
            };
        }

        public static Matrix3x4 RotateZ(float Angle)
        {
            return new Matrix3x4
            {
                M11 = (float)Math.Cos(Angle),
                M12 = -(float)Math.Sin(Angle),
                M21 = (float)Math.Sin(Angle),
                M22 = (float)Math.Cos(Angle)
            };
        }

        public static Matrix3x4 Translate(Vector3D Offset)
        {
            return new Matrix3x4
            {
                M14 = Offset.X,
                M24 = Offset.Y,
                M34 = Offset.Z
            };
        }

        public static Matrix3x4 Scale(Vector3D Scale)
        {
            return new Matrix3x4
            {
                M11 = Scale.X,
                M22 = Scale.Y,
                M33 = Scale.Z
            };
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();

            for (int Row = 0; Row < 4; Row++)
            {
                for (int Col = 0; Col < 4; Col++)
                {
                    SB.Append(string.Format("M{0}{1}: {2,-16}", Row + 1, Col + 1, this[Row, Col]));
                }

                SB.Append(Environment.NewLine);
            }

            return SB.ToString();
        }

        public string ToSerializableString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
                M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34,
                M41, M42, M43, M44);
        }

        public void Write(BinaryWriter Writer)
        {
            for (int Row = 0; Row < 4; Row++)
            {
                for (int Col = 0; Col < 4; Col++)
                {
                    Writer.Write(this[Row, Col]);
                }
            }
        }
    }
}
