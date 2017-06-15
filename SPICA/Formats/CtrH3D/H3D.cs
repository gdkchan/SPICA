using SPICA.Formats.Common;
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
        public readonly H3DPatriciaList<H3DModel>          Models;
        public readonly H3DPatriciaList<H3DMaterialParams> Materials;
        public readonly H3DPatriciaList<H3DShader>         Shaders;
        public readonly H3DPatriciaList<H3DTexture>        Textures;
        public readonly H3DPatriciaList<H3DLUT>            LUTs;
        public readonly H3DPatriciaList<H3DLight>          Lights;
        public readonly H3DPatriciaList<H3DCamera>         Cameras;
        public readonly H3DPatriciaList<H3DFog>            Fogs;
        public readonly H3DPatriciaList<H3DAnimation>      SkeletalAnimations;
        public readonly H3DPatriciaList<H3DAnimation>      MaterialAnimations;
        public readonly H3DPatriciaList<H3DAnimation>      VisibilityAnimations;
        public readonly H3DPatriciaList<H3DAnimation>      LightAnimations;
        public readonly H3DPatriciaList<H3DAnimation>      CameraAnimations;
        public readonly H3DPatriciaList<H3DAnimation>      FogAnimations;
        public readonly H3DPatriciaList<H3DScene>          Scenes;

        [Ignore] public byte BackwardCompatibility;
        [Ignore] public byte ForwardCompatibility;

        [Ignore] public ushort ConverterVersion;

        [Ignore] public H3DFlags Flags;

        public H3D()
        {
            Models               = new H3DPatriciaList<H3DModel>();
            Materials            = new H3DPatriciaList<H3DMaterialParams>();
            Shaders              = new H3DPatriciaList<H3DShader>();
            Textures             = new H3DPatriciaList<H3DTexture>();
            LUTs                 = new H3DPatriciaList<H3DLUT>();
            Lights               = new H3DPatriciaList<H3DLight>();
            Cameras              = new H3DPatriciaList<H3DCamera>();
            Fogs                 = new H3DPatriciaList<H3DFog>();
            SkeletalAnimations   = new H3DPatriciaList<H3DAnimation>();
            MaterialAnimations   = new H3DPatriciaList<H3DAnimation>();
            VisibilityAnimations = new H3DPatriciaList<H3DAnimation>();
            LightAnimations      = new H3DPatriciaList<H3DAnimation>();
            CameraAnimations     = new H3DPatriciaList<H3DAnimation>();
            FogAnimations        = new H3DPatriciaList<H3DAnimation>();
            Scenes               = new H3DPatriciaList<H3DScene>();

            BackwardCompatibility = 0x21;
            ForwardCompatibility  = 0x21;

            ConverterVersion = 42607;

            Flags = H3DFlags.IsFromNewConverter;
        }

        public static H3D Open(byte[] Data)
        {
            using (MemoryStream MS = new MemoryStream(Data))
            {
                return Open(MS);
            }
        }

        public static H3D Open(MemoryStream MS)
        {
            //Please note that data should be on Memory when opening because addresses are relocated
            //Otherwise the original file would be corrupted!
            BinaryDeserializer Deserializer = new BinaryDeserializer(MS, GetSerializationOptions());

            H3DHeader Header = Deserializer.Deserialize<H3DHeader>();

            new H3DRelocator(MS, Header).ToAbsolute();

            H3D Scene = Deserializer.Deserialize<H3D>();

            Scene.BackwardCompatibility = Header.BackwardCompatibility;
            Scene.ForwardCompatibility  = Header.ForwardCompatibility;

            Scene.ConverterVersion = Header.ConverterVersion;

            Scene.Flags = Header.Flags;

            return Scene;
        }

        public static void Save(string FileName, H3D Scene)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                uint ContentPosition = 0x44;

                if (Scene.BackwardCompatibility < 0x21) ContentPosition -= 8;
                if (Scene.BackwardCompatibility < 0x20) ContentPosition -= 4;

                FS.Seek(ContentPosition, SeekOrigin.Begin);

                H3DHeader Header = new H3DHeader();

                H3DRelocator Relocator = new H3DRelocator(FS, Header);

                BinarySerializer Serializer = new BinarySerializer(FS, GetSerializationOptions(), Relocator);

                Serializer.FileVersion = Scene.BackwardCompatibility;

                Serializer.Serialize(Scene);

                Header.Magic = "BCH";

                Header.BackwardCompatibility = Scene.BackwardCompatibility;
                Header.ForwardCompatibility  = Scene.ForwardCompatibility;

                Header.ConverterVersion = Scene.ConverterVersion;

                Header.ContentsAddress = Serializer.Contents.Info.Position;
                Header.StringsAddress  = Serializer.Strings.Info.Position;
                Header.CommandsAddress = Serializer.Commands.Info.Position;
                Header.RawDataAddress  = Serializer.RawDataTex.Info.Position;
                Header.RawExtAddress   = Serializer.RawExtTex.Info.Position;

                Header.ContentsLength = Serializer.Contents.Info.Length;
                Header.StringsLength  = Serializer.Strings.Info.Length;
                Header.CommandsLength = Serializer.Commands.Info.Length;
                Header.RawDataLength  = Serializer.RawDataTex.Info.Length;
                Header.RawExtLength   = Serializer.RawExtTex.Info.Length;

                Header.RawDataLength += Serializer.RawDataVtx.Info.Length;
                Header.RawExtLength  += Serializer.RawExtVtx.Info.Length;

                Header.UnInitDataLength     = Serializer.PhysicalAddressCount * 4;
                Header.UnInitCommandsLength = 0; //TODO: Investigate when this length is used
                Header.AddressCount         = (ushort)Serializer.PhysicalAddressCount;
                Header.Flags                = Scene.Flags;

                Relocator.ToRelative(Serializer);

                FS.Seek(0, SeekOrigin.Begin);

                Serializer.WriteValue(Header);
            }
        }

        private static SerializationOptions GetSerializationOptions()
        {
            return new SerializationOptions(LengthPos.AfterPointer, PointerType.Absolute);
        }

        public void Merge(H3D SceneData)
        {
            AddUnique(SceneData.Models,               Models);
            AddUnique(SceneData.Materials,            Materials);
            AddUnique(SceneData.Shaders,              Shaders);
            AddUnique(SceneData.Textures,             Textures);
            AddUnique(SceneData.LUTs,                 LUTs);
            AddUnique(SceneData.Lights,               Lights);
            AddUnique(SceneData.Cameras,              Cameras);
            AddUnique(SceneData.Fogs,                 Fogs);
            AddUnique(SceneData.SkeletalAnimations,   SkeletalAnimations);
            AddUnique(SceneData.MaterialAnimations,   MaterialAnimations);
            AddUnique(SceneData.VisibilityAnimations, VisibilityAnimations);
            AddUnique(SceneData.LightAnimations,      LightAnimations);
            AddUnique(SceneData.CameraAnimations,     CameraAnimations);
            AddUnique(SceneData.FogAnimations,        FogAnimations);
            AddUnique(SceneData.Scenes,               Scenes);
        }

        private void AddUnique<T>(H3DPatriciaList<T> Src, H3DPatriciaList<T> Tgt) where T : INamed
        {
            //We need to make sure that the name isn't already contained on the Tree
            //Otherwise it would throw an exception due to duplicate Keys
            foreach (T Value in Src)
            {
                string Name = Value.Name;

                int Index = 0;

                while (Tgt.Contains(Name))
                {
                    Name = $"{Value.Name}_{++Index}";
                }

                Value.Name = Name;

                Tgt.Add(Value);
            }
        }

        public void CopyMaterials()
        {
            Materials.Clear();

            foreach (H3DModel Model in Models)
            {
                foreach (H3DMaterial Material in Model.Materials)
                {
                    //Note: The IF is a workaround for multiple models with same material names
                    //This kind of problem doesn't happen on BCH, but may happen on converted formats
                    if (!Materials.Contains(Material.Name)) Materials.Add(Material.MaterialParams);
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
                Value    = string.Empty
            });

            return false;
        }
    }
}
