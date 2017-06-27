using SPICA.Math3D;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.IO;
using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxMaterialColor : ICustomSerialization
    {
        private Vector4 EmissionF;
        private Vector4 AmbientF;
        private Vector4 DiffuseF;
        private Vector4 Specular0F;
        private Vector4 Specular1F;
        private Vector4 Constant0F;
        private Vector4 Constant1F;
        private Vector4 Constant2F;
        private Vector4 Constant3F;
        private Vector4 Constant4F;
        private Vector4 Constant5F;

        public RGBA Emission;
        public RGBA Ambient;
        public RGBA Diffuse;
        public RGBA Specular0;
        public RGBA Specular1;
        public RGBA Constant0;
        public RGBA Constant1;
        public RGBA Constant2;
        public RGBA Constant3;
        public RGBA Constant4;
        public RGBA Constant5;

        private uint CommandCache;

        [Ignore] public float Scale;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Scale = AmbientF.W;
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            EmissionF  = Emission.ToVector4();
            AmbientF   = Ambient.ToVector4();
            DiffuseF   = Diffuse.ToVector4();
            Specular0F = Specular0.ToVector4();
            Specular1F = Specular1.ToVector4();
            Constant0F = Constant0.ToVector4();
            Constant1F = Constant1.ToVector4();
            Constant2F = Constant2.ToVector4();
            Constant3F = Constant3.ToVector4();
            Constant4F = Constant4.ToVector4();
            Constant5F = Constant5.ToVector4();

            AmbientF.W = Scale;

            return false;
        }

        internal byte[] GetBytes()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                WriteRGB(Writer, Emission);
                WriteRGB(Writer, Ambient);
                Writer.Write(Scale);
                WriteRGBA(Writer, Diffuse);
                WriteRGB(Writer, Specular0);
                WriteRGB(Writer, Specular1);
                WriteRGBA(Writer, Constant0);
                WriteRGBA(Writer, Constant1);
                WriteRGBA(Writer, Constant2);
                WriteRGBA(Writer, Constant3);
                WriteRGBA(Writer, Constant4);
                WriteRGBA(Writer, Constant5);

                return MS.ToArray();
            }
        }

        private void WriteRGBA(BinaryWriter Writer, RGBA Color)
        {
            Writer.Write(Color.ToVector4());
        }

        private void WriteRGB(BinaryWriter Writer, RGBA Color)
        {
            Vector4 v = Color.ToVector4();

            Writer.Write(v.X);
            Writer.Write(v.Y);
            Writer.Write(v.Z);
        }
    }
}
