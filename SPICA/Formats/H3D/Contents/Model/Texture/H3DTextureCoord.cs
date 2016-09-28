using SPICA.Math;

namespace SPICA.Formats.H3D.Contents.Model.Texture
{
    struct H3DTextureCoord
    {
        public byte Flags;
        public byte TransformType;
        public byte MappingType;
        public sbyte ReferenceCameraIndex;
        public Vector2D Scale;
        public float Rotation;
        public Vector2D Translation;
    }
}
