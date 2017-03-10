using SPICA.Math3D;
using System;

namespace SPICA.Formats.CtrH3D.Model.Material.Texture
{
    public struct H3DTextureCoord
    {
        public H3DTextureCoordFlags Flags;
        public H3DTextureTransformType TransformType;
        public H3DTextureMappingType MappingType;

        public sbyte ReferenceCameraIndex;

        public Vector2D Scale;
        public float Rotation;
        public Vector2D Translation;

        public Matrix3x4 Transform
        {
            get
            {
                Matrix3x4 Transform = new Matrix3x4();

                float SX = Scale.X;
                float SY = Scale.Y;

                float TX = Translation.X;
                float TY = Translation.Y;

                float CA = (float)Math.Cos(Rotation);
                float SA = (float)Math.Sin(Rotation);

                Transform.M11 = SX *  CA;
                Transform.M12 = SX * -SA;
                Transform.M21 = SY *  SA;
                Transform.M22 = SY *  CA;

                switch (TransformType)
                {
                    case H3DTextureTransformType.DccMaya:
                        Transform.M14 = SX * ((0.5f *  SA - 0.5f * CA) + 0.5f - TX);
                        Transform.M24 = SY * ((0.5f * -SA - 0.5f * CA) + 0.5f - TY);
                        break;

                    case H3DTextureTransformType.DccSoftImage:
                        Transform.M14 = SX * (-CA * TX - SA * TY);
                        Transform.M24 = SY * ( SA * TX - CA * TY);
                        break;

                    case H3DTextureTransformType.Dcc3dsMax:
                        Transform.M14 =
                            SX * CA * (-TX - 0.5f) -
                            SX * SA * ( TY - 0.5f) + 0.5f;
                        Transform.M24 =
                            SY * SA * (-TX - 0.5f) +
                            SY * CA * ( TY - 0.5f) + 0.5f;
                        break;
                }

                return Transform;
            }
        }
    }
}
