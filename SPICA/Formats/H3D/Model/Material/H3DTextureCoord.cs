using SPICA.Math3D;

namespace SPICA.Formats.H3D.Model.Material
{
    class H3DTextureCoord
    {
        public H3DTextureCoordFlags Flags;
        public H3DTextureTransformType TransformType;
        public H3DTextureMappingType MappingType;

        public sbyte ReferenceCameraIndex;

        public Vector2D Scale;
        public float Rotation;
        public Vector2D Translation;
    }
}
