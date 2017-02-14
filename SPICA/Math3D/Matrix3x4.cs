using SPICA.Serialization.Attributes;

using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace SPICA.Math3D
{
    [Inline]
    public class Matrix3x4
    {
        [FixedLength(4 * 3), Inline]
        private float[] Elems;

        public Matrix3x4()
        {
            Elems = new float[4 * 3];

            this[0, 0] = 1;
            this[1, 1] = 1;
            this[2, 2] = 1;
        }

        public static Matrix3x4 Empty
        {
            get
            {
                return new Matrix3x4
                {
                    M11 = 0,
                    M22 = 0,
                    M33 = 0
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

        public float this[int Row, int Col]
        {
            get { return Elems[(Row * 4) + Col]; }
            set { Elems[(Row * 4) + Col] = value; }
        }

        public static Matrix3x4 RotateX(float Angle)
        {
            return new Matrix3x4
            {
                M22 =  (float)Math.Cos(Angle),
                M23 = -(float)Math.Sin(Angle),
                M32 =  (float)Math.Sin(Angle),
                M33 =  (float)Math.Cos(Angle)
            };
        }

        public static Matrix3x4 RotateY(float Angle)
        {
            return new Matrix3x4
            {
                M11 =  (float)Math.Cos(Angle),
                M13 =  (float)Math.Sin(Angle),
                M31 = -(float)Math.Sin(Angle),
                M33 =  (float)Math.Cos(Angle)
            };
        }

        public static Matrix3x4 RotateZ(float Angle)
        {
            return new Matrix3x4
            {
                M11 =  (float)Math.Cos(Angle),
                M12 = -(float)Math.Sin(Angle),
                M21 =  (float)Math.Sin(Angle),
                M22 =  (float)Math.Cos(Angle)
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

        public static Matrix3x4 operator *(Matrix3x4 RHS, Matrix3x4 LHS)
        {
            Matrix3x4 Output = new Matrix3x4();

            Output.M11 = (LHS.M11 * RHS.M11) + (LHS.M12 * RHS.M21) + (LHS.M13 * RHS.M31);
            Output.M12 = (LHS.M11 * RHS.M12) + (LHS.M12 * RHS.M22) + (LHS.M13 * RHS.M32);
            Output.M13 = (LHS.M11 * RHS.M13) + (LHS.M12 * RHS.M23) + (LHS.M13 * RHS.M33);
            Output.M14 = (LHS.M11 * RHS.M14) + (LHS.M12 * RHS.M24) + (LHS.M13 * RHS.M34) + LHS.M14;

            Output.M21 = (LHS.M21 * RHS.M11) + (LHS.M22 * RHS.M21) + (LHS.M23 * RHS.M31);
            Output.M22 = (LHS.M21 * RHS.M12) + (LHS.M22 * RHS.M22) + (LHS.M23 * RHS.M32);
            Output.M23 = (LHS.M21 * RHS.M13) + (LHS.M22 * RHS.M23) + (LHS.M23 * RHS.M33);
            Output.M24 = (LHS.M21 * RHS.M14) + (LHS.M22 * RHS.M24) + (LHS.M23 * RHS.M34) + LHS.M24;

            Output.M31 = (LHS.M31 * RHS.M11) + (LHS.M32 * RHS.M21) + (LHS.M33 * RHS.M31);
            Output.M32 = (LHS.M31 * RHS.M12) + (LHS.M32 * RHS.M22) + (LHS.M33 * RHS.M32);
            Output.M33 = (LHS.M31 * RHS.M13) + (LHS.M32 * RHS.M23) + (LHS.M33 * RHS.M33);
            Output.M34 = (LHS.M31 * RHS.M14) + (LHS.M32 * RHS.M24) + (LHS.M33 * RHS.M34) + LHS.M34;

            return Output;
        }

        public void Invert()
        {
            Vector3D InvRot0 = new Vector3D(M11, M21, M31);
            Vector3D InvRot1 = new Vector3D(M12, M22, M32);
            Vector3D InvRot2 = new Vector3D(M13, M23, M33);

            InvRot0 *= (1f / InvRot0.Length);
            InvRot1 *= (1f / InvRot1.Length);
            InvRot2 *= (1f / InvRot2.Length);

            Vector3D Translation = new Vector3D(M14, M24, M34);

            float TranslateX = -Vector3D.Dot(InvRot0, Translation);
            float TranslateY = -Vector3D.Dot(InvRot1, Translation);
            float TranslateZ = -Vector3D.Dot(InvRot2, Translation);

            M11 = InvRot0.X;
            M12 = InvRot0.Y;
            M13 = InvRot0.Z;
            M14 = TranslateX;

            M21 = InvRot1.X;
            M22 = InvRot1.Y;
            M23 = InvRot1.Z;
            M24 = TranslateY;

            M31 = InvRot2.X;
            M32 = InvRot2.Y;
            M33 = InvRot2.Z;
            M34 = TranslateZ;
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

        public string ToSerializableString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
                M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34);
        }
    }
}
