using System.IO;
using System.Numerics;

namespace SPICA.Math3D
{
    static class MatrixExtensions
    {
        //Left-Handed Read
        public static Matrix4x4 ReadMatrix4x4(this BinaryReader Reader)
        {
            return new Matrix4x4()
            {
                M11 = Reader.ReadSingle(),
                M21 = Reader.ReadSingle(),
                M31 = Reader.ReadSingle(),
                M41 = Reader.ReadSingle(),
                M12 = Reader.ReadSingle(),
                M22 = Reader.ReadSingle(),
                M32 = Reader.ReadSingle(),
                M42 = Reader.ReadSingle(),
                M13 = Reader.ReadSingle(),
                M23 = Reader.ReadSingle(),
                M33 = Reader.ReadSingle(),
                M43 = Reader.ReadSingle(),
                M14 = Reader.ReadSingle(),
                M24 = Reader.ReadSingle(),
                M34 = Reader.ReadSingle(),
                M44 = Reader.ReadSingle()
            };
        }

        public static Matrix3x4 ReadMatrix3x4(this BinaryReader Reader)
        {
            return new Matrix3x4()
            {
                M11 = Reader.ReadSingle(),
                M21 = Reader.ReadSingle(),
                M31 = Reader.ReadSingle(),
                M41 = Reader.ReadSingle(),
                M12 = Reader.ReadSingle(),
                M22 = Reader.ReadSingle(),
                M32 = Reader.ReadSingle(),
                M42 = Reader.ReadSingle(),
                M13 = Reader.ReadSingle(),
                M23 = Reader.ReadSingle(),
                M33 = Reader.ReadSingle(),
                M43 = Reader.ReadSingle()
            };
        }

        public static Matrix3x3 ReadMatrix3x3(this BinaryReader Reader)
        {
            return new Matrix3x3()
            {
                M11 = Reader.ReadSingle(),
                M21 = Reader.ReadSingle(),
                M31 = Reader.ReadSingle(),
                M12 = Reader.ReadSingle(),
                M22 = Reader.ReadSingle(),
                M32 = Reader.ReadSingle(),
                M13 = Reader.ReadSingle(),
                M23 = Reader.ReadSingle(),
                M33 = Reader.ReadSingle()
            };
        }

        //Left-Handed Write
        public static void Write(this BinaryWriter Writer, Matrix4x4 m)
        {
            Writer.Write(m.M11);
            Writer.Write(m.M21);
            Writer.Write(m.M31);
            Writer.Write(m.M41);
            Writer.Write(m.M12);
            Writer.Write(m.M22);
            Writer.Write(m.M32);
            Writer.Write(m.M42);
            Writer.Write(m.M13);
            Writer.Write(m.M23);
            Writer.Write(m.M33);
            Writer.Write(m.M43);
            Writer.Write(m.M14);
            Writer.Write(m.M24);
            Writer.Write(m.M34);
            Writer.Write(m.M44);
        }

        public static void Write(this BinaryWriter Writer, Matrix3x4 m)
        {
            Writer.Write(m.M11);
            Writer.Write(m.M21);
            Writer.Write(m.M31);
            Writer.Write(m.M41);
            Writer.Write(m.M12);
            Writer.Write(m.M22);
            Writer.Write(m.M32);
            Writer.Write(m.M42);
            Writer.Write(m.M13);
            Writer.Write(m.M23);
            Writer.Write(m.M33);
            Writer.Write(m.M43);
        }

        public static void Write(this BinaryWriter Writer, Matrix3x3 m)
        {
            Writer.Write(m.M11);
            Writer.Write(m.M21);
            Writer.Write(m.M31);
            Writer.Write(m.M12);
            Writer.Write(m.M22);
            Writer.Write(m.M32);
            Writer.Write(m.M13);
            Writer.Write(m.M23);
            Writer.Write(m.M33);
        }

        //Right-Handed Read
        public static Matrix4x4 ReadMatrix4x4RH(this BinaryReader Reader)
        {
            return Matrix4x4.Transpose(Reader.ReadMatrix4x4());
        }

        //Right-Handed Write
        public static void WriteRH(this BinaryWriter Writer, Matrix4x4 m)
        {
            Writer.Write(Matrix4x4.Transpose(m));
        }
    }
}
