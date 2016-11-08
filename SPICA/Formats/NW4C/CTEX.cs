using SPICA.Formats.CtrH3D.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SPICA.Formats.CtrH3D
{
    public class CTEX
    {
        #region TEXTURE
        [XmlRootAttribute("NintendoWareIntermediateFile")]
        public class CtrTexture {
            public ctrMetaData MetaData = new ctrMetaData();

            public ctrGraphicsContent GraphicsContentCtr = new ctrGraphicsContent();
        }

        public class ctrGraphicsContent {
            [XmlAttribute]
            public string Namespace;

            [XmlAttribute]
            public string Version;

            [XmlArrayItem("ImageTextureCtr")]
            public List<ctrImgTex> Textures = new List<ctrImgTex>();
        }

        public class ctrImgTex {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public uint Width;

            [XmlAttribute]
            public uint Height;

            [XmlAttribute]
            public uint MipmapSize;

            [XmlAttribute]
            public string Path;

            [XmlAttribute]
            public string Encoding;

            [XmlAttribute]
            public string Format;

            [XmlArrayItem("PixelBasedImageCtr")]
            public List<ctrImage> Images = new List<ctrImage>();
        }

        public class ctrImage {
            [XmlText]
            public string data;
        }
        #endregion

        #region EDITDATA
        public class ctrMetaData {
            public ctrCreate Create = new ctrCreate();
        }

        public class ctrCreate {
            [XmlAttribute]
            public string Author;

            [XmlAttribute]
            public string Date;

            [XmlAttribute]
            public string Source;

            [XmlAttribute]
            public string FullPathOfSource;

            public ctrToolDesc ToolDescription = new ctrToolDesc();
        }

        public class ctrToolDesc {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public string Version;
        }
        #endregion

        public static void Export(object bch, string fileName, int index) {
            CtrTexture ctex = new CtrTexture();
            var ctrTexs = ((H3D)bch).Textures[index];

            //MetaData
            ctrMetaData meta = ctex.MetaData;
            meta.Create.Author = Environment.UserName;
            meta.Create.Source = Path.GetFileName(fileName);
            meta.Create.FullPathOfSource = fileName;
            meta.Create.Date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            meta.Create.ToolDescription.Name = "SPICA";
            meta.Create.ToolDescription.Version = "1.0";

            //Texture
            ctrGraphicsContent cont = ctex.GraphicsContentCtr;
            cont.Namespace = "";
            cont.Version = "0.7.0";
            ctrImgTex tex;
            ctrImage img;
            tex = new ctrImgTex();
            tex.Name = "";
            tex.Width = ctrTexs.Width;
            tex.Height = ctrTexs.Height;
            tex.MipmapSize = ctrTexs.MipmapSize;
            tex.Path = fileName;
            tex.Encoding = "Base64";
            tex.Format = ctrTexs.Format.ToString().Substring(0, 1) + ctrTexs.Format.ToString().Substring(1).ToLower(); //I'll just fix case this way instead of changing enums
            for (int i = 0; i < ctrTexs.MipmapSize; i++) {
                img = new ctrImage();
                img.data = imgToBase64(ctrTexs.RawBufferXPos);
                tex.Images.Add(img);
            }
            ctex.GraphicsContentCtr.Textures.Add(tex);

            //XML Serializer
            XmlWriterSettings settings = new XmlWriterSettings {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t"
            };
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(typeof(CtrTexture));
            XmlWriter output = XmlWriter.Create(new FileStream(fileName, FileMode.Create), settings);
            serializer.Serialize(output, ctex, ns);
            output.Close();
        }

        private static string imgToBase64(byte[] rawImg) {
            //TODO: Do mipmap stuff here
            return Convert.ToBase64String(rawImg);
        }
    }
}
