using SPICA.Math3D;

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
    }
}
