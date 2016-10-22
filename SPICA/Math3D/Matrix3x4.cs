using SPICA.Serialization.Attributes;

using System;
using System.Text;

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

            //Make identity
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

        public float M11 { get { return this[0, 0]; } set { this[0, 0] = value; } }
        public float M12 { get { return this[0, 1]; } set { this[0, 1] = value; } }
        public float M13 { get { return this[0, 2]; } set { this[0, 2] = value; } }
        public float M14 { get { return this[0, 3]; } set { this[0, 3] = value; } }

        public float M21 { get { return this[1, 0]; } set { this[1, 0] = value; } }
        public float M22 { get { return this[1, 1]; } set { this[1, 1] = value; } }
        public float M23 { get { return this[1, 2]; } set { this[1, 2] = value; } }
        public float M24 { get { return this[1, 3]; } set { this[1, 3] = value; } }

        public float M31 { get { return this[2, 0]; } set { this[2, 0] = value; } }
        public float M32 { get { return this[2, 1]; } set { this[2, 1] = value; } }
        public float M33 { get { return this[2, 2]; } set { this[2, 2] = value; } }
        public float M34 { get { return this[2, 3]; } set { this[2, 3] = value; } }

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

        //Vector3D
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

        //Vector2D
        public static Matrix3x4 Translate(Vector2D Offset)
        {
            return new Matrix3x4
            {
                M14 = Offset.X,
                M24 = Offset.Y
            };
        }

        public static Matrix3x4 Scale(Vector2D Scale)
        {
            return new Matrix3x4
            {
                M11 = Scale.X,
                M22 = Scale.Y
            };
        }

        public static Matrix3x4 operator *(Matrix3x4 LHS, Matrix3x4 RHS)
        {
            Matrix3x4 Output = new Matrix3x4();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float Sum = 0;

                    for (int k = 0; k < 3; k++)
                    {
                        Sum += LHS[i, k] * RHS[k, j];
                    }

                    Output[i, j] = Sum;
                }
            }

            return Output;
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
