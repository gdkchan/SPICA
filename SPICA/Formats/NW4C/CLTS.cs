using SPICA.Formats.H3D;
using SPICA.Formats.H3D.LUT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SPICA
{
    public class CLTS
    {
        #region LUTS
        [XmlRootAttribute("NintendoWareIntermediateFile")]
        public class CtrLUT {
            public ctrGraphicsContent GraphicsContentCtr = new ctrGraphicsContent();
        }

        public class ctrGraphicsContent {
            [XmlAttribute]
            public string Version;

            [XmlAttribute]
            public string Namespace;

            public ctrEditData EditData = new ctrEditData();

            [XmlArrayItem("LookupTableSetContentCtr")]
            public List<ctrLUTContent> LookupTableSetContents = new List<ctrLUTContent>();
        }

        public class ctrLUTContent {
            [XmlAttribute]
            public string Name;

            public ctrArrayMetaData UserData;

            public ctrLUTSet LookupTableSet = new ctrLUTSet();
        }

        public class ctrLUTSet {
            [XmlAttribute]
            public string Name;

            [XmlArrayItem("ImageLookupTableCtr")]
            public List<ctrImgLUT> Samplers = new List<ctrImgLUT>();
        }

        public class ctrImgLUT {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public bool IsGeneratedAsAbs;

            [XmlAttribute]
            public bool IsSizeDoubled;

            [XmlAttribute]
            public bool IsMipMap;

            public ctrCurveLUT CurveLookupTableCtr = new ctrCurveLUT();
        }

        public class ctrCurveLUT {
            public ctrSegFloatCurve SegmentsFloatCurve = new ctrSegFloatCurve();
        }

        public class ctrSegFloatCurve {
            [XmlAttribute]
            public string PreRepeatMethod;

            [XmlAttribute]
            public string PostRepeatMethod;

            [XmlAttribute]
            public int StartFrame;

            [XmlAttribute]
            public int EndFrame;

            public ctrSegments Segments = new ctrSegments();
        }

        public class ctrSegments {
            [XmlElement("LinearFloatSegment")]
            public List<ctrLinFloatSeg> linFloatSeg;

            [XmlElement("HermiteFloatSegment")]
            public List<ctrHermFloatSeg> hermFloatSeg;
        }

        public class ctrLinFloatSeg {
            [XmlArrayItem("LinearFloatKey")]
            public List<ctrLinFloatKey> Keys = new List<ctrLinFloatKey>();
        }

        public class ctrHermFloatSeg {
            [XmlArrayItem("HermiteFloatKey")]
            public List<ctrHermFloatKey> Keys = new List<ctrHermFloatKey>();
        }

        public class ctrLinFloatKey {
            [XmlAttribute]
            public uint Frame;

            [XmlAttribute]
            public float Value;
        }

        public class ctrHermFloatKey {
            [XmlAttribute]
            public uint Frame;

            [XmlAttribute]
            public float Value;

            [XmlAttribute]
            public uint InSlope;

            [XmlAttribute]
            public uint OutSlope;
        }
        #endregion

        #region USERDATA
        public class ctrEditData {
            public ctrMetaData MetaData = new ctrMetaData();
        }

        public class ctrMetaData {
            public string Key;

            public ctrCreate Create = new ctrCreate();

            public ctrModify Modify = new ctrModify();
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


        public class ctrModify {
            [XmlAttribute]
            public string Date;

            public ctrToolDesc ToolDescription = new ctrToolDesc();
        }

        public class ctrArrayMetaData {
            [XmlElement("IntegerArrayMetaDataXml")]
            public List<ctrIntArrayMeta> intArrayData;

            [XmlElement("FloatArrayMetaDataXml")]
            public List<ctrFloatArrayMeta> floatArrayData;

            [XmlElement("StringArrayMetaDataXml")]
            public List<ctrStringArrayMeta> strArrayData;
        }

        public class ctrIntArrayMeta {
            [XmlAttribute]
            public string DataKind;

            public string Key;

            [XmlArrayItem("IntSet")]
            public List<ctrIntSet> Values = new List<ctrIntSet>();
        }

        public class ctrIntSet {
            [XmlText]
            public string obj;
        }

        public class ctrFloatArrayMeta {
            [XmlAttribute]
            public string DataKind;

            public string Key;

            [XmlArrayItem("FloatSet")]
            public List<ctrFloatSet> Values = new List<ctrFloatSet>();
        }

        public class ctrFloatSet {
            [XmlText]
            public string obj;
        }

        public class ctrStringArrayMeta {
            [XmlAttribute]
            public string DataKind;

            public string Key;

            public string BinarizeEncoding;

            [XmlArrayItem]
            public List<ctrStringSet> Values = new List<ctrStringSet>();
        }

        public class ctrStringSet {
            [XmlText]
            public string obj;
        }
        #endregion

        public static void Export(object bch, string fileName, int index) {
            CtrLUT ctrLUT = new CtrLUT();
            var lut = ((H3D)bch).LUTs[index];
            ctrLUT.GraphicsContentCtr.Version = "1.3.0";
            ctrLUT.GraphicsContentCtr.Namespace = "";

            //EditData
            ctrEditData edit = ctrLUT.GraphicsContentCtr.EditData;
            edit.MetaData = new ctrMetaData();
            edit.MetaData.Key = "MetaData";
            edit.MetaData.Create.Author = Environment.UserName;
            edit.MetaData.Create.Source = "";
            edit.MetaData.Create.FullPathOfSource = fileName;
            edit.MetaData.Create.Date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            edit.MetaData.Create.ToolDescription.Name = "SPICA";
            edit.MetaData.Create.ToolDescription.Version = "1.0";
            edit.MetaData.Modify.Date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            edit.MetaData.Modify.ToolDescription.Name = "SPICA";
            edit.MetaData.Modify.ToolDescription.Version = "1.0";

            //LUT
            ctrImgLUT imgLut;
            ctrLUTContent lutCont = new ctrLUTContent();
            lutCont.Name = "";
            lutCont.UserData = new ctrArrayMetaData();
            
            lutCont.LookupTableSet.Name = lut.Name;
            foreach (var l in lut.Samplers) {
                //addMetaData(lutCont.UserData, l.UserData);
                imgLut = new ctrImgLUT();
                imgLut.Name = l.Name;
                imgLut.IsGeneratedAsAbs = (l.Flags & H3DLUTFlags.IsAbsolute) > 0;
                imgLut.IsSizeDoubled = true;
                imgLut.IsMipMap = false;
                imgLut.CurveLookupTableCtr.SegmentsFloatCurve.PreRepeatMethod = "None";
                imgLut.CurveLookupTableCtr.SegmentsFloatCurve.PostRepeatMethod = "None";
                imgLut.CurveLookupTableCtr.SegmentsFloatCurve.StartFrame = -256;
                imgLut.CurveLookupTableCtr.SegmentsFloatCurve.EndFrame = 256;
                imgLut.CurveLookupTableCtr.SegmentsFloatCurve.Segments = new ctrSegments();
                addKeys(ref imgLut.CurveLookupTableCtr.SegmentsFloatCurve.Segments, l.Table);
                lutCont.LookupTableSet.Samplers.Add(imgLut);
            }
            ctrLUT.GraphicsContentCtr.LookupTableSetContents.Add(lutCont);

            //XML Serializer
            XmlWriterSettings settings = new XmlWriterSettings {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t"
            };
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(typeof(CtrLUT));
            XmlWriter output = XmlWriter.Create(new FileStream(fileName, FileMode.Create), settings);
            serializer.Serialize(output, ctrLUT, ns);
            output.Close();
        }

        private static void addKeys(ref ctrSegments segments, float[] table) { //Simple, only does linear keys atm
            var segs = new List<ctrLinFloatSeg>();
            ctrLinFloatSeg linSeg = new ctrLinFloatSeg();
            ctrLinFloatKey linKey;
            for (uint k = 0; k <= 255; k++) {
                linKey = new ctrLinFloatKey();
                linKey.Frame = k;
                linKey.Value = table[k];
                linSeg.Keys.Add(linKey);
            }
            segs.Add(linSeg);
            segments.linFloatSeg = segs;
        }

        private static void addMetaData(ref ctrArrayMetaData localMeta, H3DMetaData metaData) {
            ctrIntArrayMeta intArr;
            ctrFloatArrayMeta floatArr;
            ctrStringArrayMeta strArr;
            ctrFloatSet fs;
            ctrIntSet ins;
            ctrStringSet strs;
            if (metaData == null) return;
            foreach (var val in metaData.Values) {
                if (localMeta == null) localMeta = new ctrArrayMetaData();
                if (localMeta.intArrayData == null) localMeta.intArrayData = new List<ctrIntArrayMeta>();
                if (localMeta.floatArrayData == null) localMeta.floatArrayData = new List<ctrFloatArrayMeta>();
                if (localMeta.strArrayData == null) localMeta.strArrayData = new List<ctrStringArrayMeta>();
                switch (val.Type) {
                    case H3DMetaDataType.ASCIIString:
                    case H3DMetaDataType.UnicodeString: {
                            strArr = new ctrStringArrayMeta();
                            strArr.DataKind = "StringSet";
                            strArr.Key = val.Name[0].Equals('$') ? val.Name.Substring(1) : val.Name;
                            strArr.BinarizeEncoding = "Utf16LittleEndian";
                            foreach (var vals in val.Values) {
                                strs = new ctrStringSet();
                                strs.obj = vals.ToString();
                                strArr.Values.Add(strs);
                            }
                            localMeta.strArrayData.Add(strArr);
                            break;
                        }
                    case H3DMetaDataType.Integer: {
                            intArr = new ctrIntArrayMeta();
                            intArr.DataKind = "IntSet";
                            intArr.Key = val.Name[0].Equals('$') ? val.Name.Substring(1) : val.Name;
                            foreach (var vals in val.Values) {
                                ins = new ctrIntSet();
                                ins.obj = vals.ToString();
                                intArr.Values.Add(ins);
                            }
                            localMeta.intArrayData.Add(intArr);
                            break;
                        }
                    case H3DMetaDataType.Single: {
                            floatArr = new ctrFloatArrayMeta();
                            floatArr.DataKind = "FloatSet";
                            floatArr.Key = val.Name[0].Equals('$') ? val.Name.Substring(1) : val.Name;
                            foreach (var vals in val.Values) {
                                fs = new ctrFloatSet();
                                fs.obj = vals.ToString();
                                floatArr.Values.Add(fs);
                            }
                            localMeta.floatArrayData.Add(floatArr);
                            break;
                        }
                }
            }
            if (localMeta.floatArrayData.Count == 0) localMeta.floatArrayData = null; //These 4 lines are to make sure there are no stray tags
            if (localMeta.intArrayData.Count == 0) localMeta.intArrayData = null;
            if (localMeta.strArrayData.Count == 0) localMeta.strArrayData = null;
            if (localMeta.floatArrayData == null && localMeta.intArrayData == null && localMeta.strArrayData == null) localMeta = null;
        }
    }
}
