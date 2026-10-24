using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Texture;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    [XmlRoot("COLLADA", Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
    public class DAE
    {
        [XmlAttribute] public string version = "1.4.1";

        public DAEAsset asset = new DAEAsset();

        [XmlArrayItem("animation")]    public List<DAEAnimation>   library_animations;
        [XmlArrayItem("image")]        public List<DAEImage>       library_images;
        [XmlArrayItem("material")]     public List<DAEMaterial>    library_materials;
        [XmlArrayItem("effect")]       public List<DAEEffect>      library_effects;
        [XmlArrayItem("geometry")]     public List<DAEGeometry>    library_geometries;
        [XmlArrayItem("controller")]   public List<DAEController>  library_controllers;
        [XmlArrayItem("visual_scene")] public List<DAEVisualScene> library_visual_scenes;

        public DAEScene scene = new DAEScene();

        public DAE() { }

        [XmlIgnore] public List<string> ExportWarnings { get; } = new List<string>();
        [XmlIgnore] internal readonly List<Tuple<H3DTexture, string>> ExportTextures = new List<Tuple<H3DTexture, string>>();
        [XmlIgnore] internal readonly List<Tuple<H3DTexture, string, int>> ExportTextureFaces = new List<Tuple<H3DTexture, string, int>>();
        [XmlElement("extra")] public List<DAEExtra> extra = new List<DAEExtra>();

        public DAE(H3D Scene, int MdlIndex, int AnimIndex = -1)
            : this(Scene, MdlIndex, AnimIndex, new DAEExportOptions()) { }

        public DAE(H3D Scene, int MdlIndex, int AnimIndex, DAEExportOptions Options)
        {
            new DAEExporter(this, Scene, MdlIndex, AnimIndex, Options).Export();
        }

        public void Save(string FileName)
        {
            string DirectoryName = Path.GetDirectoryName(Path.GetFullPath(FileName));

            foreach (Tuple<H3DTexture, string> Texture in ExportTextures)
            {
                using (System.Drawing.Bitmap Bitmap = Texture.Item1.ToBitmap())
                {
                    Bitmap.Save(Path.Combine(DirectoryName, Texture.Item2), System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            foreach (Tuple<H3DTexture, string, int> Texture in ExportTextureFaces)
            {
                using (System.Drawing.Bitmap Bitmap = Texture.Item1.ToBitmap(Texture.Item3))
                {
                    Bitmap.Save(Path.Combine(DirectoryName, Texture.Item2), System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(DAE));

                Serializer.Serialize(FS, this);
            }

            if (ExportWarnings.Count > 0)
            {
                File.WriteAllLines(FileName + ".warnings.txt", ExportWarnings);
            }
        }

    }
}
