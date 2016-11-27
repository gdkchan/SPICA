using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Camera;
using SPICA.Formats.CtrH3D.Fog;
using SPICA.Formats.CtrH3D.Light;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Scene;
using SPICA.Formats.CtrH3D.Shader;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System.IO;

namespace SPICA.Formats.CtrH3D
{
    public class H3D : ICustomSerialization
    {
        public PatriciaList<H3DModel>          Models;
        public PatriciaList<H3DMaterialParams> Materials;
        public PatriciaList<H3DShader>         Shaders;
        public PatriciaList<H3DTexture>        Textures;
        public PatriciaList<H3DLUT>            LUTs;
        public PatriciaList<H3DLight>          Lights;
        public PatriciaList<H3DCamera>         Cameras;
        public PatriciaList<H3DFog>            Fogs;
        public PatriciaList<H3DAnimation>      SkeletalAnimations;
        public PatriciaList<H3DAnimation>      MaterialAnimations;
        public PatriciaList<H3DAnimation>      VisibilityAnimations;
        public PatriciaList<H3DAnimation>      LightAnimations;
        public PatriciaList<H3DAnimation>      CameraAnimations;
        public PatriciaList<H3DAnimation>      FogAnimations;
        public PatriciaList<H3DScene>          Scenes;

        [Ignore] public ushort ConverterVersion;

        [Ignore] public byte BackwardCompatibility;
        [Ignore] public byte ForwardCompatibility;

        [Ignore] public H3DFlags Flags;

        public H3D()
        {
            Models               = new PatriciaList<H3DModel>();
            Materials            = new PatriciaList<H3DMaterialParams>();
            Shaders              = new PatriciaList<H3DShader>();
            Textures             = new PatriciaList<H3DTexture>();
            LUTs                 = new PatriciaList<H3DLUT>();
            Lights               = new PatriciaList<H3DLight>();
            Cameras              = new PatriciaList<H3DCamera>();
            Fogs                 = new PatriciaList<H3DFog>();
            SkeletalAnimations   = new PatriciaList<H3DAnimation>();
            MaterialAnimations   = new PatriciaList<H3DAnimation>();
            VisibilityAnimations = new PatriciaList<H3DAnimation>();
            LightAnimations      = new PatriciaList<H3DAnimation>();
            CameraAnimations     = new PatriciaList<H3DAnimation>();
            FogAnimations        = new PatriciaList<H3DAnimation>();
            Scenes               = new PatriciaList<H3DScene>();
        }

        public static H3D Open(string FileName)
        {
            if (File.Exists(FileName))
            {
                using (MemoryStream MS = new MemoryStream(File.ReadAllBytes(FileName)))
                {
                    return Open(MS);
                }
            }
            else
            {
                throw new FileNotFoundException(string.Format("The file \"{0}\" was not found!", FileName));
            }
        }

        public static H3D Open(MemoryStream MS)
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

        public void CopyMaterials()
        {
            Materials.Clear();

            foreach (H3DModel Model in Models)
            {
                foreach (H3DMaterial Material in Model.Materials)
                {
                    Materials.Add(Material.MaterialParams);
                }
            }
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer) { }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //The original tool seems to this empty name for some reason
            Serializer.Strings.Values.Add(new RefValue
            {
                Position = -1,
                Value = string.Empty
            });

            return false;
        }
    }
}
