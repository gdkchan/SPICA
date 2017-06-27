using SPICA.Math3D;

using System.IO;
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

        private uint Flags; //Enabled/Dirty, set by game, SBZ

        public Matrix3x4 Transform;

        internal byte[] GetBytes(bool IsUninitialized)
        {
            /*
             * When the Texture Coord isn't used, Scale and Translation isn't included in the hash.
             * The reason for this is because those two are treated as reference types, even through
             * they are serialized as value types. We can't calculate the hash for a reference type
             * with equal to nullptr, so those are skipped.
             */
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write(SourceCoordIndex);

                Writer.Write((uint)MappingType);

                Writer.Write(ReferenceCameraIndex);

                Writer.Write((uint)TransformType);

                if (!IsUninitialized)
                {
                    Writer.Write(Scale);
                }

                Writer.Write(Rotation);

                if (!IsUninitialized)
                {
                    Writer.Write(Translation);
                }

                Writer.Write((byte)0);

                Writer.Write(Transform);

                return MS.ToArray();
            }
        }
    }
}
