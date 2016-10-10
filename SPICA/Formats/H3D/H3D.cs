using SPICA.Formats.H3D.Animation;
using SPICA.Formats.H3D.Camera;
using SPICA.Formats.H3D.Fog;
using SPICA.Formats.H3D.Light;
using SPICA.Formats.H3D.LUT;
using SPICA.Formats.H3D.Model;
using SPICA.Formats.H3D.Model.Material;
using SPICA.Formats.H3D.Scene;
using SPICA.Formats.H3D.Shader;
using SPICA.Formats.H3D.Texture;
using SPICA.Serialization;

using System.IO;

namespace SPICA.Formats.H3D
{
    class H3D
    {
        public PatriciaPointersList<H3DModel> Models;
        public PatriciaPointersList<H3DMaterialParams> Materials;
        public PatriciaPointersList<H3DShader> Shaders;
        public PatriciaPointersList<H3DTexture> Textures;
        public PatriciaPointersList<H3DLUT> LUTs;
        public PatriciaPointersList<H3DLight> Lights;
        public PatriciaPointersList<H3DCamera> Cameras;
        public PatriciaPointersList<H3DFog> Fogs;
        public PatriciaPointersList<H3DAnimation> SkeletalAnimations;
        public PatriciaPointersList<H3DAnimation> MaterialAnimations;
        public PatriciaPointersList<H3DAnimation> VisibilityAnimations;
        public PatriciaPointersList<H3DAnimation> LightAnimations;
        public PatriciaPointersList<H3DAnimation> CameraAnimations;
        public PatriciaPointersList<H3DAnimation> FogAnimations;
        public PatriciaPointersList<H3DScene> Scenes;

        public static H3D Open(string FileName)
        {
            using (MemoryStream MS = new MemoryStream(File.ReadAllBytes(FileName)))
            {
                BinaryDeserializer Deserializer = new BinaryDeserializer(MS);

                new H3DRelocator(MS, Deserializer.Deserialize<H3DHeader>()).ToAbsolute();

                return Deserializer.Deserialize<H3D>();
            }
        }

        public static void Save(string FileName, H3D Data)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                FS.Seek(0x44, SeekOrigin.Begin);

                BinarySerializer Serializer = new BinarySerializer(FS);

                Serializer.Serialize(Data);
            }
        }
    }
}
