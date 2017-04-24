using SPICA.Math3D;

using System;
using System.Numerics;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    public struct H3DTextureCoord
    {
        public H3DTextureCoordFlags    Flags;
        public H3DTextureTransformType TransformType;
        public H3DTextureMappingType   MappingType;

        public sbyte ReferenceCameraIndex;

        public Vector2 Scale;
        public float   Rotation;
        public Vector2 Translation;

        public Matrix3x4 Transform
        {
            get
            {
                Matrix3x4 Transform = Matrix3x4.Identity;

                float SX = Scale.X;
                float SY = Scale.Y;

                float TX = Translation.X;
                float TY = Translation.Y;

                float CA = (float)Math.Cos(Rotation);
                float SA = (float)Math.Sin(Rotation);

                Transform.M11 = SX *  CA;
                Transform.M12 = SY *  SA;
                Transform.M21 = SX * -SA;
                Transform.M22 = SY *  CA;

                switch (TransformType)
                {
                    case H3DTextureTransformType.DccMaya:
                        Transform.M41 = SX * ((0.5f *  SA - 0.5f * CA) + 0.5f - TX);
                        Transform.M42 = SY * ((0.5f * -SA - 0.5f * CA) + 0.5f - TY);
                        break;

                    case H3DTextureTransformType.DccSoftImage:
                        Transform.M41 = SX * (-CA * TX - SA * TY);
                        Transform.M42 = SY * ( SA * TX - CA * TY);
                        break;

                    case H3DTextureTransformType.Dcc3dsMax:
                        Transform.M41 =
                            SX * CA * (-TX - 0.5f) -
                            SX * SA * ( TY - 0.5f) + 0.5f;
                        Transform.M42 =
                            SY * SA * (-TX - 0.5f) +
                            SY * CA * ( TY - 0.5f) + 0.5f;
                        break;
                }

                return Transform;
            }
        }
    }
}
