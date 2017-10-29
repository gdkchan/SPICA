using SPICA.Formats.Common;
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

        public Matrix3x4 GetTransform()
        {
            return TextureTransform.GetTransform(
                Scale,
                Rotation,
                Translation,
                (TextureTransformType)TransformType);
        }
    }
}
