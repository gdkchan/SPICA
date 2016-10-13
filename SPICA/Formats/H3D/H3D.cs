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
using SPICA.Serialization.Serializer;

using System;
using System.IO;

namespace SPICA.Formats.H3D
{
    class H3D : ICustomSerialization
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

        [NonSerialized]
        public ushort ConverterVersion;

        [NonSerialized]
        public byte BackwardCompatibility;

        [NonSerialized]
        public byte ForwardCompatibility;

        [NonSerialized]
        public H3DFlags Flags;

        public static H3D Open(string FileName)
        {
            if (File.Exists(FileName))
            {
                using (MemoryStream MS = new MemoryStream(File.ReadAllBytes(FileName)))
                {
                    BinaryDeserializer Deserializer = new BinaryDeserializer(MS);
                    H3DHeader Header = Deserializer.Deserialize<H3DHeader>();

                    new H3DRelocator(MS, Header).ToAbsolute();

                    H3D Model = Deserializer.Deserialize<H3D>();

                    Model.ConverterVersion = Header.ConverterVersion;

                    Model.BackwardCompatibility = Header.BackwardCompatibility;
                    Model.ForwardCompatibility = Header.ForwardCompatibility;

                    Model.Flags = Header.Flags;

                    return Model;
                }
            }
            else
            {
                throw new FileNotFoundException(string.Format("The file \"{0}\" was not found!", FileName));
            }
        }

        public static void Save(string FileName, H3D Model)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                FS.Seek(0x44, SeekOrigin.Begin);

                H3DHeader Header = new H3DHeader();

                H3DRelocator Relocator = new H3DRelocator(FS, Header);

                BinarySerializer Serializer = new BinarySerializer(FS, Relocator);

                Serializer.Serialize(Model);

                Header.Magic = "BCH";

                Header.ConverterVersion = Model.ConverterVersion;

                Header.BackwardCompatibility = Model.BackwardCompatibility;
                Header.ForwardCompatibility = Model.ForwardCompatibility;

                Header.ContentsAddress = Serializer.Contents.Info.Position;
                Header.StringsAddress = Serializer.Strings.Info.Position;
                Header.CommandsAddress = Serializer.Commands.Info.Position;
                Header.RawDataAddress = Serializer.RawDataTex.Info.Position;
                Header.RawExtAddress = Serializer.RawExtTex.Info.Position;

                Header.ContentsLength = Serializer.Contents.Info.Length;
                Header.StringsLength = Serializer.Strings.Info.Length;
                Header.CommandsLength = Serializer.Commands.Info.Length;
                Header.RawDataLength = Serializer.RawDataTex.Info.Length;
                Header.RawExtLength = Serializer.RawExtTex.Info.Length;

                Header.RawDataLength += Serializer.RawDataVtx.Info.Length;
                Header.RawExtLength += Serializer.RawExtVtx.Info.Length;

                Header.UnInitDataLength = Serializer.PhysicalAddressCount * 4;
                Header.AddressCount = (ushort)Serializer.PhysicalAddressCount;

                Header.Flags = Model.Flags;

                Relocator.ToRelative(Serializer);

                FS.Seek(0, SeekOrigin.Begin);

                Serializer.WriteObject(Header);
            }
        }

        public void Deserialize(BinaryDeserializer Deserializer) { }

        public bool Serialize(BinarySerializer Serializer)
        {
            //The original tool seems to this empty name for some reason
            Serializer.Strings.Values.Add(new RefValue
            {
                Position = -1,
                Value = string.Empty
            });

            return false;
        }

        public static void Export(H3D Data, string FileName) {
            CMDL.export(Data, FileName, 0);
        }
    }
}
