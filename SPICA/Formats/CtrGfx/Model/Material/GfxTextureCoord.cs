using SPICA.Math3D;

using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxTextureCoord
    {
        public int SourceCoordIndex;
        
        public GfxTextureMappingType MappingType;

        public int ReferenceCameraIndex;

        public GfxTextureTransformType TransformType;

        public Vector2 Scale;
        public float   Rotation;
        public Vector2 Translation;

        private uint Flags; //Enabled/Dirty

        public Matrix3x4 Transform;
    }
}
