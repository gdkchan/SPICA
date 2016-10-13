using SPICA.Formats.H3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using SPICA.Formats.H3D.Model;
using SPICA.Formats.H3D.Model.Material;
using SPICA.Formats.H3D.Model.Mesh;
using SPICA.PICA.Commands;
using System.Globalization;
using SPICA.Math3D;

namespace SPICA.Serialization
{
    public class CMDL
    {
        #region MODELS
        [XmlRootAttribute("NintendoWareIntermediateFile")]
        public class CtrModel {
            public ctrGraphicsContent GraphicsContentCtr = new ctrGraphicsContent();
        }

        public class ctrGraphicsContent {
            [XmlAttribute]
            public string Version = "1.3.0";

            [XmlAttribute]
            public string Namespace = "";

            public ctrEditData EditData = new ctrEditData();
            public ctrModels Models = new ctrModels();
        }

        public class ctrModels {
            public ctrSkeletalModel SkeletalModel = new ctrSkeletalModel();
        }
        #endregion

        #region USERDATA
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

        #region EDITDATA
        public class ctrEditData { //This class is used in multiple locations with different elements, so init them manually
            public ctrMetaData MetaData = null;

            public ctrModelDccToolExportOpt ModelDccToolExportOption = null;

            public ctrOptLogArrayMeta OptimizationLogArrayMetaData = null;

            public ctrContentSummaryMeta ContentSummaryMetaData = null;

            public ctrDccSrcNode DccToolSourceNodeMetaData = null;
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

        public class ctrDccSrcNode {
            public string Key;

            [XmlArrayItem("NodeName")]
            public List<ctrNodeName> Values = new List<ctrNodeName>();
        }

        public class ctrNodeName {
            [XmlText]
            public string Name;
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

        public class ctrContentSummaryMeta {
            public string Key;
            public ctrValues Values = new ctrValues();

        }

        public class ctrValues {
            public ctrContentSummary ContentSummary = new ctrContentSummary();
        }

        public class ctrContentSummary {
            [XmlAttribute]
            public string ContentTypeName;

            [XmlArrayItem("ObjectSummary")]
            public List<ctrObjectSummary> ObjectSummaries = new List<ctrObjectSummary>();
        }

        public class ctrObjectSummary {
            [XmlAttribute]
            public string TypeName;

            [XmlAttribute]
            public string Name;

            [XmlArrayItem("Note")]
            public List<ctrNote> Notes = new List<ctrNote>();
        }

        public class ctrNote {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public uint Value;
        }

        public class ctrOptLogArrayMeta {
            [XmlAttribute]
            public uint Size;

            public string Key;

            [XmlArrayItem("OptimizationLog")]
            public List<ctrOptLog> Values = new List<ctrOptLog>();
        }

        public class ctrOptLog {
            [XmlAttribute]
            public string Date;

            [XmlAttribute]
            public string EditorVersion;

            [XmlAttribute]
            public double OptimizePrimitiveAverageCacheMissRatio;

            [XmlAttribute]
            public string OptimizerIdentifier;

            public ctrOpt Options = new ctrOpt();
        }

        public class ctrOpt {
            [XmlAttribute]
            public string NodeCompressionMode;

            [XmlAttribute]
            public bool IsMergeMeshOwnerBoneEnabled;

            [XmlAttribute]
            public bool IsCombineMeshEnabled;

            [XmlAttribute]
            public bool IsCompressMaterialEnabled;

            [XmlAttribute]
            public bool IsOptimizePlygonPrimitiveEnabled;

            [XmlAttribute]
            public bool IsConvertOneBoneSkeletalModelToModel;

            [XmlAttribute]
            public bool IsDeleteUnusedVertexEnabled;

            [XmlAttribute]
            public string PositionQuantizeMode;

            [XmlAttribute]
            public string NormalQuantizeMode;

            [XmlAttribute]
            public string TextureQuantizeMode;

            [XmlAttribute]
            public bool GroupByIndexStream;

            public int OptimizePolygonPrimitiveLevel;
        }

        public class ctrModelDccToolExportOpt {
            [XmlAttribute]
            public uint ExportStartFrame;

            [XmlAttribute]
            public uint Magnify;

            [XmlAttribute]
            public string AdjustSkinning;

            [XmlAttribute]
            public string MeshVisibilityMode;

            public string Key;
        }
        #endregion

        #region SKELETON
        public class ctrSkeletalModel {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public bool IsBranchVisible;

            [XmlAttribute]
            public bool IsVisible;

            [XmlAttribute]
            public string CullingMode;

            [XmlAttribute]
            public bool IsNonuniformScalable;

            [XmlAttribute]
            public uint LayerId;

            [XmlAttribute]
            public uint NeededBoneCapacity;

            public ctrArrayMetaData UserData = null;

            public ctrEditData EditData = null;

            [XmlArrayItem("GraphicsAnimationGroupDescription")]
            public List<ctrGraphicsAnimGroupDesc> AnimationGroupDescriptions = new List<ctrGraphicsAnimGroupDesc>();

            public ctrTransform Transform = new ctrTransform();

            [XmlArrayItem("SeparateDataShapeCtr")]
            public List<ctrSeparateDataShape> Shapes = new List<ctrSeparateDataShape>();

            [XmlArrayItem("MaterialCtr")]
            public List<ctrMaterial> Materials = new List<ctrMaterial>();

            [XmlArrayItem("Mesh")]
            public List<ctrMesh> Meshes = new List<ctrMesh>();

            [XmlArrayItem("MeshNodeVisibility")]
            public List<ctrMeshVis> MeshNodeVisibilities = new List<ctrMeshVis>();

            public ctrSkeleton Skeleton = new ctrSkeleton();
        }

        public class ctrSkeleton {
            [XmlAttribute]
            public string RootBoneName;

            [XmlAttribute]
            public string ScalingRule;

            [XmlAttribute]
            public bool IsTranslateAnimationEnabled;

            [XmlArrayItem("Bone")]
            public List<ctrBone> Bones = new List<ctrBone>();
        }

        public class ctrBone {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public string ParentBoneName;

            [XmlAttribute]
            public bool IsSegmentScaleCompensate;

            [XmlAttribute]
            public bool IsCompressible;

            [XmlAttribute]
            public bool IsNeededRendering;

            [XmlAttribute]
            public bool HasSkinningMatrix;

            [XmlAttribute]
            public string BillboardMode;

            public ctrArrayMetaData EditData = null;

            public ctrArrayMetaData UserData = null;

            public ctrTransform Transform = new ctrTransform();
        }
        #endregion

        #region ANIMATIONS
        public class ctrGraphicsAnimGroupDesc {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public string EvaluationTiming;

            [XmlArrayItem("AnimationMemberDescription")]
            public List<ctrAnimMemberDesc> MemberInformationSet = new List<ctrAnimMemberDesc>();
        }

        public class ctrAnimMemberDesc {
            [XmlAttribute]
            public string BlendOperationName;

            [XmlAttribute]
            public bool IsBinarized;

            public string Path;
        }
        #endregion

        #region TRANSFORM
        public class ctrTransform {
            public ctrScale Scale = new ctrScale();
            public ctrRotate Rotate = new ctrRotate();
            public ctrTranslate Translate = new ctrTranslate();
        }

        public class ctrScale {
            [XmlAttribute]
            public float X;

            [XmlAttribute]
            public float Y;

            [XmlAttribute]
            public float Z;
        }

        public class ctrRotate {
            [XmlAttribute]
            public float X;

            [XmlAttribute]
            public float Y;

            [XmlAttribute]
            public float Z;
        }

        public class ctrTranslate {
            [XmlAttribute]
            public float X;

            [XmlAttribute]
            public float Y;

            [XmlAttribute]
            public float Z;
        }
        #endregion

        #region SHAPES
        public class ctrSeparateDataShape {
            public ctrOBB OrientedBoundingBox = null;

            public ctrPositionOff PositionOffset = new ctrPositionOff();

            [XmlArrayItem("PrimitiveSetCtr")]
            public List<ctrPrimSet> PrimitiveSets = new List<ctrPrimSet>();

            public ctrVertAttrib VertexAttributes = new ctrVertAttrib();
        }

        public class ctrOBB {
            public ctrCenterPos CenterPosition = new ctrCenterPos();

            public ctrMatrix OrientationMatrix = new ctrMatrix();

            public ctrSize Size = new ctrSize();
        }

        public class ctrCenterPos {
            [XmlAttribute]
            public float X;

            [XmlAttribute]
            public float Y;

            [XmlAttribute]
            public float Z;
        }

        public class ctrMatrix {
            [XmlAttribute]
            public float M00;

            [XmlAttribute]
            public float M01;

            [XmlAttribute]
            public float M02;

            [XmlAttribute]
            public float M10;

            [XmlAttribute]
            public float M11;

            [XmlAttribute]
            public float M12;

            [XmlAttribute]
            public float M20;

            [XmlAttribute]
            public float M21;

            [XmlAttribute]
            public float M22;
        }

        public class ctrSize {
            [XmlAttribute]
            public float X;

            [XmlAttribute]
            public float Y;

            [XmlAttribute]
            public float Z;
        }

        public class ctrPositionOff {
            [XmlAttribute]
            public float X;

            [XmlAttribute]
            public float Y;

            [XmlAttribute]
            public float Z;
        }

        public class ctrPrimSet {
            [XmlAttribute]
            public string SkinningMode;

            public string BoneIndexTable;

            [XmlArrayItem("PrimitiveCtr")]
            public List<ctrPrim> Primitives = new List<ctrPrim>();
        }

        public class ctrPrim {
            public ctrIndexStreams IndexStreams = new ctrIndexStreams();
        }

        public class ctrIndexStreams {
            [XmlElement("UshortIndexStreamCtr")]
            public List<ctrIndexStream> ushortIndexStreams = null;

            [XmlElement("UbyteIndexStreamCtr")]
            public List<ctrIndexStream> ubyteIndexStreams = null;
        }

        public class ctrIndexStream {
            [XmlAttribute]
            public string PrimitiveMode;

            [XmlAttribute]
            public int Size;

            [XmlText]
            public string stream;
        }

        public class ctrVertAttrib {
            [XmlElement("Vector3VertexStreamCtr")]
            public List<ctrVertStream> Vec3Attributes;

            [XmlElement("Vector2VertexStreamCtr")]
            public List<ctrVertStream> Vec2Attributes;

            [XmlElement("Vector4VertexStreamCtr")]
            public List<ctrVertStream> Vec4Attributes;
        }

        public class ctrVertStream {
            [XmlAttribute]
            public string Usage;

            [XmlAttribute]
            public int VertexSize;

            [XmlAttribute]
            public float Scale;

            [XmlAttribute]
            public string QuantizedMode;

            [XmlText]
            public string VecArray;
        }
        #endregion

        #region MATERIALS
        public class ctrMaterial {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public bool IsCompressible;

            [XmlAttribute]
            public uint LightSetIndex;

            [XmlAttribute]
            public uint FogIndex;

            [XmlAttribute]
            public bool IsFragmentLightEnabled;

            [XmlAttribute]
            public bool IsVertexLightEnabled;

            [XmlAttribute]
            public bool IsHemiSphereLightEnabled;

            [XmlAttribute]
            public bool IsHemiSphereOcclusionEnabled;

            [XmlAttribute]
            public bool IsFogEnabled;

            [XmlAttribute]
            public string TextureCoordinateConfig;

            [XmlAttribute]
            public string TranslucencyKind;

            [XmlAttribute]
            public int ShaderProgramDescriptionIndex;

            [XmlAttribute]
            public string ShaderBinaryKind;

            public ctrArrayMetaData UserData = null;

            public ctrArrayMetaData EditData = null;

            public string ShaderReference;

            public ctrMatColor MaterialColor = new ctrMatColor();

            public ctrRaster Rasterization = new ctrRaster();

            [XmlArrayItem("TextureCoordinatorCtr")]
            public List<ctrTexCoordinator> TextureCoordinators = new List<ctrTexCoordinator>();

            [XmlArrayItem("PixelBasedTextureMapperCtr")]
            public List<ctrPixTexMap> TextureMappers = new List<ctrPixTexMap>();

            public ctrFragShader FragmentShader = new ctrFragShader();

            public ctrFragOp FragmentOperation = new ctrFragOp();
        }

        public class ctrMatColor {
            [XmlAttribute]
            public float VertexColorScale;

            public ctrColor Emission = new ctrColor();

            public ctrColor Ambient = new ctrColor();

            public ctrColor Diffuse = new ctrColor();

            public ctrColor Specular0 = new ctrColor();

            public ctrColor Specular1 = new ctrColor();

            public ctrColor Constant0 = new ctrColor();

            public ctrColor Constant1 = new ctrColor();

            public ctrColor Constant2 = new ctrColor();

            public ctrColor Constant3 = new ctrColor();

            public ctrColor Constant4 = new ctrColor();

            public ctrColor Constant5 = new ctrColor();
        }

        public class ctrColor {
            [XmlAttribute]
            public float R;

            [XmlAttribute]
            public float G;

            [XmlAttribute]
            public float B;

            [XmlAttribute]
            public float A;
        }

        public class ctrRaster {
            [XmlAttribute]
            public string CullingMode;

            [XmlAttribute]
            public bool IsPolygonOffsetEnabled;

            [XmlAttribute]
            public float PolygonOffsetUnit;
        }

        public class ctrTexCoordinator {
            [XmlAttribute]
            public int SourceCoordinate;

            [XmlAttribute]
            public string MappingMethod;

            [XmlAttribute]
            public int ReferenceCamera;

            [XmlAttribute]
            public string MatrixMode;

            [XmlAttribute]
            public float ScaleS;

            [XmlAttribute]
            public float ScaleT;

            [XmlAttribute]
            public float Rotate;

            [XmlAttribute]
            public float TranslateS;

            [XmlAttribute]
            public float TranslateT;
        }

        public class ctrPixTexMap {
            public string TextureReference;

            public ctrTexSampler StandardTextureSamplerCtr = new ctrTexSampler();
        }

        public class ctrTexSampler {
            [XmlAttribute]
            public string MinFilter;

            [XmlAttribute]
            public string MagFilter;

            [XmlAttribute]
            public string WrapS;

            [XmlAttribute]
            public string WrapT;

            [XmlAttribute]
            public int MinLod;

            [XmlAttribute]
            public float LodBias;

            public ctrColor BorderColor = new ctrColor();
        }

        public class ctrFragShader {
            [XmlAttribute]
            public string LayerConfig;

            public ctrColor BufferColor = new ctrColor();

            public ctrFragBump FragmentBump = new ctrFragBump();

            public ctrFragLighting FragmentLighting = new ctrFragLighting();

            public ctrFragLightTable FragmentLightingTable = new ctrFragLightTable();

            [XmlArrayItem("TextureCombinerCtr")]
            public List<ctrTexCombine> TextureCombiners = new List<ctrTexCombine>();

            public ctrAlphaTest AlphaTest = new ctrAlphaTest();
        }

        public class ctrFragBump {
            [XmlAttribute]
            public string BumpTextureIndex;

            [XmlAttribute]
            public string BumpMode;

            [XmlAttribute]
            public bool IsBumpRenormalize;
        }

        public class ctrFragLighting {
            [XmlAttribute]
            public string FresnelConfig;

            [XmlAttribute]
            public bool IsClampHighLight;

            [XmlAttribute]
            public bool IsDistribution0Enabled;

            [XmlAttribute]
            public bool IsDistribution1Enabled;

            [XmlAttribute]
            public bool IsGeometricFactor0Enabled;

            [XmlAttribute]
            public bool IsGeometricFactor1Enabled;

            [XmlAttribute]
            public bool IsReflectionEnabled;
        }

        public class ctrFragLightTable {
            public ctrSampler ReflectanceRSampler = new ctrSampler();

            public ctrSampler ReflectanceGSampler = new ctrSampler();

            public ctrSampler ReflectanceBSampler = new ctrSampler();

            public ctrSampler Distribution0Sampler = new ctrSampler();

            public ctrSampler Distribution1Sampler = new ctrSampler();

            public ctrSampler FresnelSampler = new ctrSampler();
        }

        public class ctrSampler {
            [XmlAttribute]
            public bool IsAbs;

            [XmlAttribute]
            public string Input;

            [XmlAttribute]
            public string Scale;

            public string NullLookupTableCtr;
        }

        public class ctrTexCombine {
            [XmlAttribute]
            public string CombineRgb;

            [XmlAttribute]
            public string CombineAlpha;

            [XmlAttribute]
            public string ScaleRgb;

            [XmlAttribute]
            public string ScaleAlpha;

            [XmlAttribute]
            public string Constant;

            [XmlAttribute]
            public string BufferInputRgb;

            [XmlAttribute]
            public string BufferInputAlpha;

            public ctrCombSrc SourceRgb = new ctrCombSrc();

            public ctrCombOp OperandRgb = new ctrCombOp();

            public ctrCombSrc SourceAlpha = new ctrCombSrc();

            public ctrCombOp OperandAlpha = new ctrCombOp();
        }

        public class ctrCombSrc {
            [XmlAttribute]
            public string Source0;

            [XmlAttribute]
            public string Source1;

            [XmlAttribute]
            public string Source2;
        }

        public class ctrCombOp {
            [XmlAttribute]
            public string Operand0;

            [XmlAttribute]
            public string Operand1;

            [XmlAttribute]
            public string Operand2;
        }

        public class ctrAlphaTest {
            [XmlAttribute]
            public bool IsTestEnabled;

            [XmlAttribute]
            public string TestFunction;

            [XmlAttribute]
            public int TestReference;
        }

        public class ctrFragOp {
            public ctrFragDepthOp DepthOperation = new ctrFragDepthOp();

            public ctrFragBlendOp BlendOperation = new ctrFragBlendOp();

            public ctrFragStencilOp StencilOperation = new ctrFragStencilOp();
        }

        public class ctrFragDepthOp {
            [XmlAttribute]
            public bool IsTestEnabled;

            [XmlAttribute]
            public string TestFunction;

            [XmlAttribute]
            public bool IsMaskEnabled;
        }

        public class ctrFragBlendOp {
            [XmlAttribute]
            public string Mode;

            [XmlAttribute]
            public string LogicOperation;

            public ctrBlendParam RgbParameter = new ctrBlendParam();

            public ctrBlendParam AlphaParameter = new ctrBlendParam();

            public ctrColor BlendColor = new ctrColor();
        }

        public class ctrFragStencilOp {
            [XmlAttribute]
            public bool IsTestEnabled;

            [XmlAttribute]
            public string TestFunction;

            [XmlAttribute]
            public int TestReference;

            [XmlAttribute]
            public byte TestMask;

            [XmlAttribute]
            public string FailOperation;

            [XmlAttribute]
            public string ZFailOperation;

            [XmlAttribute]
            public string PassOperation;
        }

        public class ctrBlendParam {
            [XmlAttribute]
            public string BlendFunctionSource;

            [XmlAttribute]
            public string BlendFunctionDestination;

            [XmlAttribute]
            public string BlendEquation;
        }
        #endregion

        #region MESHES
        public class ctrMesh {
            [XmlAttribute]
            public bool IsVisible;

            [XmlAttribute]
            public int RenderPriority;

            [XmlAttribute]
            public string MeshNodeName;

            public ctrEditData EditData;

            public string SeparateShapeReference;

            public string MaterialReference;
        }

        public class ctrMeshVis {
            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public bool IsVisible;
        }
        #endregion

        /// <summary>
        ///     Exports a Model to the CMDL format.
        /// </summary>
        /// <param name="model">The Model that will be exported</param>
        /// <param name="fileName">The output File Name</param>
        /// <param name="skeletalAnimationIndex">(Optional) Index of the skeletal animation.</param>
        public static void export(object model, string fileName, int index, int skeletalAnimationIndex = -1) {
            CtrModel ctrMdl = new CtrModel();
            H3DModel mdl = ((H3D)model).Models[index];

            //EditData
            ctrEditData edit = ctrMdl.GraphicsContentCtr.EditData;
            edit.MetaData = new ctrMetaData();
            edit.MetaData.Key = "MetaData";
            edit.MetaData.Create.Author = Environment.UserName;
            edit.MetaData.Create.Source = "";
            edit.MetaData.Create.FullPathOfSource = "";
            edit.MetaData.Create.Date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            edit.MetaData.Create.ToolDescription.Name = "SPICA";
            edit.MetaData.Create.ToolDescription.Version = "1.0";
            edit.MetaData.Modify.Date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            edit.MetaData.Modify.ToolDescription.Name = "SPICA";
            edit.MetaData.Modify.ToolDescription.Version = "1.0";
            edit.ContentSummaryMetaData = new ctrContentSummaryMeta();
            edit.ContentSummaryMetaData.Key = "ContentSummaries";
            edit.ContentSummaryMetaData.Values.ContentSummary.ContentTypeName = "GraphicsContent";

            List<ctrObjectSummary> summaries = edit.ContentSummaryMetaData.Values.ContentSummary.ObjectSummaries;
            ctrObjectSummary sum = new ctrObjectSummary();
            sum.TypeName = "SkeletalModel";
            sum.Name = mdl.Name;
            summaries.Add(sum);

            List<ctrNote> notes = sum.Notes;
            ctrNote note;
            int skinNone = 0,
                skinRigid = 0,
                skinSmooth = 0,
                subMeshes = 0,
                indices = 0,
                verts = 0;
            foreach (var m in mdl.Meshes) {
                verts += m.GetVertices().Count();
                
                foreach (var s in m.SubMeshes) {
                    subMeshes++;
                    indices += s.Indices.Count();
                    if (s.Skinning == H3DSubMeshSkinning.None) skinNone++;
                    if (s.Skinning == H3DSubMeshSkinning.Rigid) skinRigid++;
                    if (s.Skinning == H3DSubMeshSkinning.Smooth) skinSmooth++;
                }
            }
            note = new ctrNote();
            note.Name = "MaterialCount";
            note.Value = (uint)mdl.Materials.Contents.Count;
            notes.Add(note);
            note = new ctrNote();
            note.Name = "ShapeCount";
            note.Value = (uint)mdl.Meshes.Count();
            notes.Add(note);
            note = new ctrNote();
            note.Name = "MeshCount";
            note.Value = (uint)mdl.Meshes.Count();
            notes.Add(note);
            note = new ctrNote();
            note.Name = "BoneCount";
            note.Value = (uint)mdl.Skeleton.Contents.Count;
            notes.Add(note);
            note = new ctrNote();
            note.Name = "TotalPrimitiveSetCount";
            note.Value = (uint)(skinNone + skinRigid + skinSmooth);
            notes.Add(note);
            note = new ctrNote();
            note.Name = "TotalNoneSkinningPrimitiveSetCount";
            note.Value = (uint)skinNone;
            notes.Add(note);
            note = new ctrNote();
            note.Name = "TotalRigidSkinningPrimitiveSetCount";
            note.Value = (uint)skinRigid;
            notes.Add(note);
            note = new ctrNote();
            note.Name = "TotalSmoothSkinningPrimitiveSetCount";
            note.Value = (uint)skinSmooth;
            notes.Add(note);
            note = new ctrNote();
            note.Name = "TotalIndexStreamCount";
            note.Value = (uint)subMeshes;
            notes.Add(note);
            note = new ctrNote();
            note.Name = "TotalPolygonCount";
            note.Value = (uint)indices / 3;
            notes.Add(note);
            note = new ctrNote();
            note.Name = "TotalVertexCount";
            note.Value = (uint)verts;
            notes.Add(note);

            //Model data
            ctrSkeletalModel skelModel = ctrMdl.GraphicsContentCtr.Models.SkeletalModel;
            addMetaData(ref skelModel.UserData, mdl.MetaData);
            skelModel.EditData = new ctrEditData();
            skelModel.EditData.ModelDccToolExportOption = new ctrModelDccToolExportOpt();
            skelModel.EditData.OptimizationLogArrayMetaData = new ctrOptLogArrayMeta();
            skelModel.Name = mdl.Name;
            skelModel.IsBranchVisible = true;
            skelModel.IsVisible = true;
            skelModel.CullingMode = "Dynamic";
            skelModel.IsNonuniformScalable = false;
            skelModel.LayerId = 0;
            skelModel.NeededBoneCapacity = 20;

            //Model UserData
            ctrArrayMetaData modelUserData = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.UserData;
            addMetaData(ref modelUserData, mdl.MetaData);

            //Model EditData
            ctrModelDccToolExportOpt dccExpOpt = skelModel.EditData.ModelDccToolExportOption;
            dccExpOpt.ExportStartFrame = 0;
            dccExpOpt.Magnify = 1;
            dccExpOpt.AdjustSkinning = "SmoothSkinning";
            dccExpOpt.MeshVisibilityMode = "BindByName";
            dccExpOpt.Key = "ModelDccToolInfo";
            ctrOptLogArrayMeta optLogArr = skelModel.EditData.OptimizationLogArrayMetaData;
            optLogArr.Size = 1;
            optLogArr.Key = "OptimizationLogs";
            ctrOptLog optLog;
            optLog = new ctrOptLog();
            optLog.Date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            optLog.EditorVersion = "1.4.5.44775";
            optLog.OptimizePrimitiveAverageCacheMissRatio = 0.7663249;
            optLog.OptimizerIdentifier = "AlgorithmCombo";
            optLog.Options.NodeCompressionMode = "CullLeafBone";
            optLog.Options.IsMergeMeshOwnerBoneEnabled = false;
            optLog.Options.IsCombineMeshEnabled = true;
            optLog.Options.IsCompressMaterialEnabled = false;
            optLog.Options.IsOptimizePlygonPrimitiveEnabled = true;
            optLog.Options.IsConvertOneBoneSkeletalModelToModel = false;
            optLog.Options.IsDeleteUnusedVertexEnabled = false;
            optLog.Options.PositionQuantizeMode = "Byte";
            optLog.Options.NormalQuantizeMode = "Byte";
            optLog.Options.TextureQuantizeMode = "Byte";
            optLog.Options.GroupByIndexStream = false;
            optLog.Options.OptimizePolygonPrimitiveLevel = 0;
            optLogArr.Values.Add(optLog);

            //Skeleton Animation
            List<ctrGraphicsAnimGroupDesc> animGroups = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.AnimationGroupDescriptions;
            ctrGraphicsAnimGroupDesc animGroup;
            ctrAnimMemberDesc animMembDesc;
            animGroup = new ctrGraphicsAnimGroupDesc();
            animGroup.Name = "SkeletalAnimation";
            animGroup.EvaluationTiming = "AfterSceneCulling";
            animGroups.Add(animGroup);
            List<ctrAnimMemberDesc> skelMembSet = animGroup.MemberInformationSet;
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "CalculatedTransform";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Skeleton.Bones[\"*\"].AnimatedTransform";
            skelMembSet.Add(animMembDesc);

            //Visibility Animation
            animGroup = new ctrGraphicsAnimGroupDesc();
            animGroup.Name = "VisibilityAnimation";
            animGroup.EvaluationTiming = "BeforeWorldUpdate";
            animGroups.Add(animGroup);
            List<ctrAnimMemberDesc> visMembSet = animGroup.MemberInformationSet;
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "Bool";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "IsVisible";
            visMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "Bool";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Meshes[\"*\"].IsVisible";
            visMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "Bool";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "MeshNodeVisibilities[\"*\"].IsVisible";
            visMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "Bool";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "IsBranchVisible";
            visMembSet.Add(animMembDesc);

            //Material Animation
            animGroup = new ctrGraphicsAnimGroupDesc();
            animGroup.Name = "MaterialAnimation";
            animGroup.EvaluationTiming = "AfterSceneCulling";
            animGroups.Add(animGroup);
            List<ctrAnimMemberDesc> matMembSet = animGroup.MemberInformationSet;
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Emission";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Ambient";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Diffuse";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Specular0";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Specular1";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Constant0";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Constant1";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Constant2";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Constant3";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Constant4";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].MaterialColor.Constant5";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].TextureMappers[\"*\"].Sampler.BorderColor";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "Int";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].TextureMappers[\"*\"].Texture";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "RgbaColor";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].FragmentOperation.BlendOperation.BlendColor";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "Vector2";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].TextureCoordinators[\"*\"].Scale";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "Float";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].TextureCoordinators[\"*\"].Rotate";
            matMembSet.Add(animMembDesc);
            animMembDesc = new ctrAnimMemberDesc();
            animMembDesc.BlendOperationName = "Vector2";
            animMembDesc.IsBinarized = true;
            animMembDesc.Path = "Materials[\"*\"].TextureCoordinators[\"*\"].Translate";
            matMembSet.Add(animMembDesc);

            //Transform
            ctrTransform trans = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.Transform;
            trans.Scale.X = 1;
            trans.Scale.Y = 1;
            trans.Scale.Z = 1;
            trans.Rotate.X = 0;
            trans.Rotate.Y = 0;
            trans.Rotate.Z = 0;
            trans.Translate.X = 0;
            trans.Translate.Y = 0;
            trans.Translate.Z = 0;
            
            //Shapes
            List<ctrSeparateDataShape> shapes = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.Shapes;
            ctrSeparateDataShape shape;
            ctrOBB obb;
            ctrPrimSet primSet = null;
            ctrPrim prim;
            ctrVertAttrib vertAtt = null;
            List<ctrVertStream> streams = null;
            ctrVertStream vertStream = null;
            StringBuilder sb;
            ctrIndexStream indStream;
            foreach (var sh in mdl.Meshes) {
                shape = new ctrSeparateDataShape();
                if (sh.MetaData != null) {
                    foreach (var md in sh.MetaData.Values) {
                        switch (md.Type) {
                            case H3DMetaDataType.BoundingBox: {
                                    H3DBoundingBox obbox = (H3DBoundingBox)md[0];
                                    obb = new ctrOBB();
                                    obb.CenterPosition.X = obbox.Center.X;
                                    obb.CenterPosition.Y = obbox.Center.Y;
                                    obb.CenterPosition.Z = obbox.Center.Z;
                                    obb.OrientationMatrix.M00 = obbox.Orientation.Elems[0];
                                    obb.OrientationMatrix.M01 = obbox.Orientation.Elems[1];
                                    obb.OrientationMatrix.M02 = obbox.Orientation.Elems[2];
                                    obb.OrientationMatrix.M10 = obbox.Orientation.Elems[3];
                                    obb.OrientationMatrix.M11 = obbox.Orientation.Elems[4];
                                    obb.OrientationMatrix.M12 = obbox.Orientation.Elems[5];
                                    obb.OrientationMatrix.M20 = obbox.Orientation.Elems[6];
                                    obb.OrientationMatrix.M21 = obbox.Orientation.Elems[7];
                                    obb.OrientationMatrix.M22 = obbox.Orientation.Elems[8];
                                    obb.Size.X = obbox.Size.X;
                                    obb.Size.Y = obbox.Size.Y;
                                    obb.Size.Z = obbox.Size.Z;
                                    shape.OrientedBoundingBox = obb;
                                    break;
                                }
                        }
                    }
                }
                shape.PositionOffset.X = sh.PositionOffset.X;
                shape.PositionOffset.Y = sh.PositionOffset.Y;
                shape.PositionOffset.Z = sh.PositionOffset.Z;

                //Create each PrimitiveSetCtr from submeshes
                foreach (var sub in sh.SubMeshes) {
                    string skinMode = "";
                    switch (sub.Skinning) {
                        case H3DSubMeshSkinning.None:
                            skinMode = "None";
                            break;
                        case H3DSubMeshSkinning.Rigid:
                            skinMode = "RigidSkinning";
                            break;
                        case H3DSubMeshSkinning.Smooth:
                            skinMode = "SmoothSkinning";
                            break;
                    }
                    primSet = new ctrPrimSet();
                    primSet.SkinningMode = skinMode;
                    var boneInd = new List<ushort>();
                    for (int bi = 0; bi < sub.BoneIndicesCount; bi++) boneInd.Add(sub.BoneIndices[bi]);
                    primSet.BoneIndexTable = string.Join(" ", boneInd);
                    //Serialize indices 
                    prim = new ctrPrim();
                    if (sub.MaxIndex > 255) {
                        if (prim.IndexStreams.ushortIndexStreams == null) prim.IndexStreams.ushortIndexStreams = new List<ctrIndexStream>();
                        indStream = new ctrIndexStream();
                        indStream.PrimitiveMode = "Triangles";
                        indStream.Size = sub.Indices.Count();
                        indStream.stream = string.Join(" ", sub.Indices);
                        prim.IndexStreams.ushortIndexStreams.Add(indStream);
                    } else {
                        if (prim.IndexStreams.ubyteIndexStreams == null) prim.IndexStreams.ubyteIndexStreams = new List<ctrIndexStream>();
                        indStream = new ctrIndexStream();
                        indStream.PrimitiveMode = "Triangles";
                        indStream.Size = sub.Indices.Count();
                        indStream.stream = string.Join(" ", sub.Indices);
                        prim.IndexStreams.ubyteIndexStreams.Add(indStream);
                    }
                    primSet.Primitives.Add(prim);
                    shape.PrimitiveSets.Add(primSet);
                }
                //Create each vertex attribute
                shape.VertexAttributes.Vec2Attributes = new List<ctrVertStream>();
                shape.VertexAttributes.Vec3Attributes = new List<ctrVertStream>();
                shape.VertexAttributes.Vec4Attributes = new List<ctrVertStream>();
                foreach (var att in sh.Attributes) {
                    vertStream = new ctrVertStream(); //stream
                    vertStream.Usage = att.Name.ToString();
                    vertStream.VertexSize = sh.GetVertices().Count();
                    vertStream.Scale = att.Scale;
                    vertStream.QuantizedMode = att.Format.ToString();
                    switch (att.Name) {
                        case PICAAttributeName.Position: 
                            {
                                vertStream.VecArray = genVec3Array(sh.GetVertices(), att.Format, att.Name);
                                shape.VertexAttributes.Vec3Attributes.Add(vertStream);
                                break;
                            }
                        case PICAAttributeName.Normal: 
                            {
                                vertStream.VecArray = genVec3Array(sh.GetVertices(), att.Format, att.Name);
                                shape.VertexAttributes.Vec3Attributes.Add(vertStream);
                                break;
                            }
                        case PICAAttributeName.TextureCoordinate0: 
                            {
                                vertStream.VecArray = genVec2Array(sh.GetVertices(), att.Format, att.Name);
                                shape.VertexAttributes.Vec2Attributes.Add(vertStream);
                                break;
                            }
                        case PICAAttributeName.BoneIndex: 
                            {
                                vertStream.VecArray = genBoneIndex(sh.GetVertices());
                                shape.VertexAttributes.Vec4Attributes.Add(vertStream);
                                break;
                            }
                        case PICAAttributeName.BoneWeight:
                            {
                                vertStream.VecArray = genBoneWeights(sh.GetVertices());
                                shape.VertexAttributes.Vec4Attributes.Add(vertStream);
                                break;
                            }
                    }
                    
                }
                shapes.Add(shape);
            }

            //Materials
            List<ctrMaterial> mats = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.Materials;
            ctrMaterial mat;
            ctrMatColor matCol;
            ctrRaster raster;
            List<ctrTexCoordinator> texCoords;
            ctrTexCoordinator texCoord;
            List<ctrPixTexMap> texMaps;
            ctrPixTexMap texMap;
            ctrTexSampler texSamp;
            ctrFragShader fragShade;
            ctrTexCombine texComb;
            ctrFragOp fragOp;
            foreach (var mt in mdl.Materials.Contents) {
                mat = new ctrMaterial();
                mat.Name = mt.Name;
                mat.IsCompressible = true;
                mat.LightSetIndex = mt.MaterialParams.LightSetIndex;
                mat.FogIndex = mt.MaterialParams.FogIndex;
                mat.IsFragmentLightEnabled = H3DMaterialFlags.IsFragmentLightingEnabled > 0;
                mat.IsVertexLightEnabled = H3DMaterialFlags.IsVertexLightingEnabled > 0;
                mat.IsHemiSphereLightEnabled = H3DMaterialFlags.IsHemiSphereLightingEnabled > 0;
                mat.IsHemiSphereOcclusionEnabled = H3DMaterialFlags.IsHemiSphereOcclusionEnabled > 0;
                mat.IsFogEnabled = H3DMaterialFlags.IsFogEnabled > 0;
                mat.TextureCoordinateConfig = "Config0120";
                mat.TranslucencyKind = "Layer0";
                mat.ShaderProgramDescriptionIndex = -1;
                mat.ShaderBinaryKind = "Default";
                addMetaData(ref mat.UserData, mt.MaterialParams.MetaData);
                //ShaderReference
                string shdr = mt.MaterialParams.ShaderReference;
                mat.ShaderReference = shdr == "0@DefaultShader" ? "" : shdr;
                //MaterialColor
                matCol = new ctrMatColor();
                matCol.VertexColorScale = mt.MaterialParams.ColorScale;
                matCol.Emission.R = mt.MaterialParams.EmissionColor.R / 255;
                matCol.Emission.G = mt.MaterialParams.EmissionColor.G / 255;
                matCol.Emission.B = mt.MaterialParams.EmissionColor.B / 255;
                matCol.Emission.A = mt.MaterialParams.EmissionColor.A / 255;
                matCol.Ambient.R = mt.MaterialParams.AmbientColor.R / 255;
                matCol.Ambient.G = mt.MaterialParams.AmbientColor.G / 255;
                matCol.Ambient.B = mt.MaterialParams.AmbientColor.B / 255;
                matCol.Ambient.A = mt.MaterialParams.AmbientColor.A / 255;
                matCol.Diffuse.R = mt.MaterialParams.DiffuseColor.R / 255;
                matCol.Diffuse.G = mt.MaterialParams.DiffuseColor.G / 255;
                matCol.Diffuse.B = mt.MaterialParams.DiffuseColor.B / 255;
                matCol.Diffuse.A = mt.MaterialParams.DiffuseColor.A / 255;
                matCol.Specular0.R = mt.MaterialParams.Specular0.R / 255;
                matCol.Specular0.G = mt.MaterialParams.Specular0.G / 255;
                matCol.Specular0.B = mt.MaterialParams.Specular0.B / 255;
                matCol.Specular0.A = mt.MaterialParams.Specular0.A / 255;
                matCol.Specular1.R = mt.MaterialParams.Specular1.R / 255;
                matCol.Specular1.G = mt.MaterialParams.Specular1.G / 255;
                matCol.Specular1.B = mt.MaterialParams.Specular1.B / 255;
                matCol.Specular1.A = mt.MaterialParams.Specular1.A / 255;
                matCol.Constant0.R = mt.MaterialParams.Constant0.R / 255;
                matCol.Constant0.G = mt.MaterialParams.Constant0.G / 255;
                matCol.Constant0.B = mt.MaterialParams.Constant0.B / 255;
                matCol.Constant0.A = mt.MaterialParams.Constant0.A / 255;
                matCol.Constant1.R = mt.MaterialParams.Constant1.R / 255;
                matCol.Constant1.G = mt.MaterialParams.Constant1.G / 255;
                matCol.Constant1.B = mt.MaterialParams.Constant1.B / 255;
                matCol.Constant1.A = mt.MaterialParams.Constant1.A / 255;
                matCol.Constant2.R = mt.MaterialParams.Constant2.R / 255;
                matCol.Constant2.G = mt.MaterialParams.Constant2.G / 255;
                matCol.Constant2.B = mt.MaterialParams.Constant2.B / 255;
                matCol.Constant2.A = mt.MaterialParams.Constant2.A / 255;
                matCol.Constant3.R = mt.MaterialParams.Constant3.R / 255;
                matCol.Constant3.G = mt.MaterialParams.Constant3.G / 255;
                matCol.Constant3.B = mt.MaterialParams.Constant3.B / 255;
                matCol.Constant3.A = mt.MaterialParams.Constant3.A / 255;
                matCol.Constant4.R = mt.MaterialParams.Constant4.R / 255;
                matCol.Constant4.G = mt.MaterialParams.Constant4.G / 255;
                matCol.Constant4.B = mt.MaterialParams.Constant4.B / 255;
                matCol.Constant4.A = mt.MaterialParams.Constant4.A / 255;
                matCol.Constant5.R = mt.MaterialParams.Constant5.R / 255;
                matCol.Constant5.G = mt.MaterialParams.Constant5.G / 255;
                matCol.Constant5.B = mt.MaterialParams.Constant5.B / 255;
                matCol.Constant5.A = mt.MaterialParams.Constant5.A / 255;
                mat.MaterialColor = matCol;
                //Rasterization
                raster = new ctrRaster();
                raster.CullingMode = mt.MaterialParams.FaceCulling.ToString();
                raster.IsPolygonOffsetEnabled = mt.MaterialParams.PolygonOffsetUnit > 0;
                raster.PolygonOffsetUnit = mt.MaterialParams.PolygonOffsetUnit;
                mat.Rasterization = raster;
                //TextureCoordinators
                texCoords = new List<ctrTexCoordinator>();
                mat.TextureCoordinators = texCoords;
                int numOfTextures = 0; //Hacky way to check how many textures the GPU is using.
                if (mt.Texture0Name != null) numOfTextures++;
                if (mt.Texture1Name != null) numOfTextures++;
                if (mt.Texture2Name != null) numOfTextures++;
                for (int g = 0; g < numOfTextures; g++) {
                    texCoord = new ctrTexCoordinator();
                    texCoord.SourceCoordinate = 0;
                    texCoord.MappingMethod = mt.MaterialParams.TextureCoords[g].MappingType.ToString();
                    texCoord.ReferenceCamera = mt.MaterialParams.TextureCoords[g].ReferenceCameraIndex;
                    texCoord.MatrixMode = mt.MaterialParams.TextureCoords[g].TransformType.ToString();
                    texCoord.ScaleS = mt.MaterialParams.TextureCoords[g].Scale.X;
                    texCoord.ScaleT = mt.MaterialParams.TextureCoords[g].Scale.Y;
                    texCoord.Rotate = mt.MaterialParams.TextureCoords[g].Rotation;
                    texCoord.TranslateS = mt.MaterialParams.TextureCoords[g].Translation.X;
                    texCoord.TranslateT = mt.MaterialParams.TextureCoords[g].Translation.Y;
                    texCoords.Add(texCoord);
                }
                //TextureMappers
                texMaps = new List<ctrPixTexMap>();
                mat.TextureMappers = texMaps;
                for (int g = 0; g < numOfTextures; g++) {
                    texMap = new ctrPixTexMap();
                    texMap.TextureReference = "Textures[\"" + mt.Name + "\"]";
                    texSamp = new ctrTexSampler();
                    texSamp.MinFilter = mt.TextureMappers[g].MinFilter.ToString();
                    texSamp.MagFilter = mt.TextureMappers[g].MagFilter.ToString();
                    texSamp.WrapS = mt.TextureMappers[g].WrapU.ToString();
                    texSamp.WrapT = mt.TextureMappers[g].WrapV.ToString();
                    texSamp.MinLod = mt.TextureMappers[g].MinLOD;
                    texSamp.LodBias = mt.TextureMappers[g].LODBias;
                    texSamp.BorderColor.R = mt.TextureMappers[g].BorderColor.R;
                    texSamp.BorderColor.G = mt.TextureMappers[g].BorderColor.G;
                    texSamp.BorderColor.B = mt.TextureMappers[g].BorderColor.B;
                    texSamp.BorderColor.A = mt.TextureMappers[g].BorderColor.A;
                    texMap.StandardTextureSamplerCtr = texSamp;
                    texMaps.Add(texMap);
                }
                //FragmentShader
                fragShade = new ctrFragShader();
                string bumpMode = "",
                       fresnelConf = "";
                switch (mt.MaterialParams.BumpMode) {
                    case 0:
                        bumpMode = "NotUsed";
                        break;
                    case 1:
                        bumpMode = "AsBump";
                        break;
                    case 2:
                        bumpMode = "AsTangent";
                        break;
                }
                switch (mt.MaterialParams.FresnelConfig) {
                    case 0:
                        fresnelConf = "No";
                        break;
                    case 1:
                        fresnelConf = "Pri";
                        break;
                    case 2:
                        fresnelConf = "Sec";
                        break;
                    case 3:
                        fresnelConf = "PriSec";
                        break;
                }
                fragShade.LayerConfig = "ConfigurationType" + mt.MaterialParams.LayerConfig.ToString();
                fragShade.BufferColor.R = mt.MaterialParams.TexEnvBufferColor.R;
                fragShade.BufferColor.G = mt.MaterialParams.TexEnvBufferColor.G;
                fragShade.BufferColor.B = mt.MaterialParams.TexEnvBufferColor.B;
                fragShade.BufferColor.A = mt.MaterialParams.TexEnvBufferColor.A;
                fragShade.FragmentBump.BumpTextureIndex = "Texture" + mt.MaterialParams.BumpTexture;
                fragShade.FragmentBump.BumpMode = bumpMode;

                H3DFragLightFlags lightFlags = mt.MaterialParams.FragmentLightFlags;
                fragShade.FragmentBump.IsBumpRenormalize = (lightFlags & H3DFragLightFlags.IsBumpRenormalizeEnabled) > 0;
                fragShade.FragmentLighting.FresnelConfig = fresnelConf;
                fragShade.FragmentLighting.IsClampHighLight = (lightFlags & H3DFragLightFlags.IsClampHighLightEnabled) > 0;
                fragShade.FragmentLighting.IsDistribution0Enabled = (lightFlags & H3DFragLightFlags.IsLUTDist0Enabled) > 0;
                fragShade.FragmentLighting.IsDistribution1Enabled = (lightFlags & H3DFragLightFlags.IsLUTDist1Enabled) > 0;
                fragShade.FragmentLighting.IsGeometricFactor0Enabled = (lightFlags & H3DFragLightFlags.IsLUTGeoFactor0Enabled) > 0;
                fragShade.FragmentLighting.IsGeometricFactor1Enabled = (lightFlags & H3DFragLightFlags.IsLUTGeoFactor1Enabled) > 0;
                fragShade.FragmentLighting.IsReflectionEnabled = (lightFlags & H3DFragLightFlags.IsLUTReflectionEnabled) > 0;

                fragShade.FragmentLightingTable.ReflectanceRSampler.IsAbs = true;   //TODO: Fill all this out with real data
                fragShade.FragmentLightingTable.ReflectanceRSampler.Input = "CosNormalHalf";
                fragShade.FragmentLightingTable.ReflectanceRSampler.Scale = "One";
                fragShade.FragmentLightingTable.ReflectanceRSampler.NullLookupTableCtr = "";
                fragShade.FragmentLightingTable.ReflectanceGSampler.IsAbs = true;
                fragShade.FragmentLightingTable.ReflectanceGSampler.Input = "CosNormalHalf";
                fragShade.FragmentLightingTable.ReflectanceGSampler.Scale = "One";
                fragShade.FragmentLightingTable.ReflectanceGSampler.NullLookupTableCtr = "";
                fragShade.FragmentLightingTable.ReflectanceBSampler.IsAbs = true;
                fragShade.FragmentLightingTable.ReflectanceBSampler.Input = "CosNormalHalf";
                fragShade.FragmentLightingTable.ReflectanceBSampler.Scale = "One";
                fragShade.FragmentLightingTable.ReflectanceBSampler.NullLookupTableCtr = "";
                fragShade.FragmentLightingTable.Distribution0Sampler.IsAbs = true;
                fragShade.FragmentLightingTable.Distribution0Sampler.Input = "CosNormalHalf";
                fragShade.FragmentLightingTable.Distribution0Sampler.Scale = "One";
                fragShade.FragmentLightingTable.Distribution0Sampler.NullLookupTableCtr = "";
                fragShade.FragmentLightingTable.Distribution1Sampler.IsAbs = true;
                fragShade.FragmentLightingTable.Distribution1Sampler.Input = "CosNormalHalf";
                fragShade.FragmentLightingTable.Distribution1Sampler.Scale = "One";
                fragShade.FragmentLightingTable.Distribution1Sampler.NullLookupTableCtr = "";
                fragShade.FragmentLightingTable.FresnelSampler.IsAbs = true;
                fragShade.FragmentLightingTable.FresnelSampler.Input = "CosNormalHalf";
                fragShade.FragmentLightingTable.FresnelSampler.Scale = "One";
                fragShade.FragmentLightingTable.FresnelSampler.NullLookupTableCtr = "";

                foreach (var comb in mt.MaterialParams.TexEnvStages) {
                    texComb = new ctrTexCombine();
                    texComb.CombineRgb = comb.Combiner.RGBCombiner.ToString();
                    texComb.CombineAlpha = comb.Combiner.AlphaCombiner.ToString();
                    texComb.ScaleRgb = comb.Scale.RGBScale.ToString();
                    texComb.ScaleAlpha = comb.Scale.AlphaScale.ToString();
                    texComb.Constant = "Constant0";
                    texComb.BufferInputRgb = "PreviousBuffer";
                    texComb.BufferInputAlpha = "PreviousBuffer";
                    texComb.SourceRgb.Source0 = comb.Source.RGBSource[0].ToString();
                    texComb.SourceRgb.Source1 = comb.Source.RGBSource[1].ToString();
                    texComb.SourceRgb.Source2 = comb.Source.RGBSource[2].ToString();
                    texComb.SourceAlpha.Source0 = comb.Source.AlphaSource[0].ToString();
                    texComb.SourceAlpha.Source1 = comb.Source.AlphaSource[1].ToString();
                    texComb.SourceAlpha.Source2 = comb.Source.AlphaSource[2].ToString();
                    texComb.OperandRgb.Operand0 = comb.Operand.RGBOp[0].ToString();
                    texComb.OperandRgb.Operand1 = comb.Operand.RGBOp[1].ToString();
                    texComb.OperandRgb.Operand2 = comb.Operand.RGBOp[2].ToString();
                    texComb.OperandAlpha.Operand0 = comb.Operand.AlphaOp[0].ToString();
                    texComb.OperandAlpha.Operand1 = comb.Operand.AlphaOp[1].ToString();
                    texComb.OperandAlpha.Operand2 = comb.Operand.AlphaOp[2].ToString();
                    fragShade.TextureCombiners.Add(texComb);
                }
                
                fragShade.AlphaTest.IsTestEnabled = mt.MaterialParams.FragmentAlphaTest.Enabled;
                fragShade.AlphaTest.TestFunction = getTestFunc(mt.MaterialParams.FragmentAlphaTest.Function);
                fragShade.AlphaTest.TestReference = mt.MaterialParams.FragmentAlphaTest.Reference;

                mat.FragmentShader = fragShade;

                //FragmentOperations
                fragOp = new ctrFragOp();
                fragOp.DepthOperation.IsTestEnabled = true;
                fragOp.DepthOperation.TestFunction = "Less";
                fragOp.DepthOperation.IsMaskEnabled = mt.MaterialParams.DepthColorMask.Enabled;
                fragOp.BlendOperation.Mode = "NotUsed";
                fragOp.BlendOperation.LogicOperation = mt.MaterialParams.LogicalOperation.ToString();

                fragOp.BlendOperation.RgbParameter.BlendFunctionSource = mt.MaterialParams.BlendingFunction.RGBSourceFunc.ToString();
                fragOp.BlendOperation.RgbParameter.BlendFunctionDestination = mt.MaterialParams.BlendingFunction.RGBDestFunc.ToString();
                fragOp.BlendOperation.RgbParameter.BlendEquation = mt.MaterialParams.BlendingFunction.RGBEquation.ToString();

                fragOp.BlendOperation.AlphaParameter.BlendFunctionSource = mt.MaterialParams.BlendingFunction.AlphaSourceFunc.ToString();
                fragOp.BlendOperation.AlphaParameter.BlendFunctionDestination = mt.MaterialParams.BlendingFunction.AlphaDestFunc.ToString();
                fragOp.BlendOperation.AlphaParameter.BlendEquation = mt.MaterialParams.BlendingFunction.AlphaEquation.ToString();

                fragOp.BlendOperation.BlendColor.R = mt.MaterialParams.BlendColor.R;
                fragOp.BlendOperation.BlendColor.G = mt.MaterialParams.BlendColor.G;
                fragOp.BlendOperation.BlendColor.B = mt.MaterialParams.BlendColor.B;
                fragOp.BlendOperation.BlendColor.A = mt.MaterialParams.BlendColor.A;

                fragOp.StencilOperation.IsTestEnabled = mt.MaterialParams.StencilTest.Enabled;
                fragOp.StencilOperation.TestFunction = getTestFunc(mt.MaterialParams.StencilTest.Function);
                fragOp.StencilOperation.TestReference = mt.MaterialParams.StencilTest.Reference;
                fragOp.StencilOperation.TestMask = mt.MaterialParams.StencilTest.Mask;
                fragOp.StencilOperation.FailOperation = mt.MaterialParams.StencilOperation.FailOp.ToString();
                fragOp.StencilOperation.ZFailOperation = mt.MaterialParams.StencilOperation.ZFailOp.ToString();
                fragOp.StencilOperation.PassOperation = mt.MaterialParams.StencilOperation.ZPassOp.ToString();

                mat.FragmentOperation = fragOp;

                mats.Add(mat);
            }

            //Meshes
            List<ctrMesh> meshes = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.Meshes;
            ctrMesh mesh;
            int i;
            for (i = 0; i < mdl.Meshes.Count; i++) {
                mesh = new ctrMesh();
                mesh.IsVisible = true;
                mesh.RenderPriority = mdl.Meshes[i].Key;
                mesh.MeshNodeName = mdl.MeshesTree.Nodes[mdl.Meshes[i].NodeId+1].Name;
                mesh.SeparateShapeReference = "Shapes[" + i + "]";
                mesh.MaterialReference = "Materials[\"" + mdl.Materials.Tree[mdl.Meshes[i].NodeId].Name + "\"]";
                addDccToolMeta(ref mesh.EditData, mdl.MeshesTree.Nodes[mdl.Meshes[i].NodeId + 1].Name);
                meshes.Add(mesh);
            }

            //Mesh Node Visibility
            List<ctrMeshVis> meshVisibilites = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.MeshNodeVisibilities;
            ctrMeshVis meshVisibility;
            for (i = 0; i < mdl.MeshesCount; i++) {
                meshVisibility = new ctrMeshVis();
                meshVisibility.Name = mdl.MeshesTree.Nodes[i+1].Name;
                meshVisibility.IsVisible = mdl.MeshVisibilities[i];
                meshVisibilites.Add(meshVisibility);
            }

            //Skeleton
            ctrSkeleton skeleton = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.Skeleton;
            List<ctrBone> bones = new List<ctrBone>();
            ctrBone bone;
            ctrFloatArrayMeta fmeta;
            ctrFloatSet fset;
            skeleton.RootBoneName = mdl.Skeleton[0].Name; //Not sure if this always holds true
            skeleton.ScalingRule = mdl.SkeletonScalingType.ToString();
            skeleton.IsTranslateAnimationEnabled = true;
            skeleton.Bones = bones;
            foreach (var b in mdl.Skeleton) {
                bone = new ctrBone();
                bone.Name = b.Name;
                bone.ParentBoneName = b.ParentIndex == -1 ? "" : mdl.Skeleton[b.ParentIndex].Name;
                bone.IsSegmentScaleCompensate = (b.Flags & H3DBoneFlags.IsSegmentScaleCompensate) != 0;
                bone.IsCompressible = true; //Not used I think
                bone.IsNeededRendering = b.Name == "Root" ? false : true; //Not used I think
                bone.HasSkinningMatrix = b.Name == "Root" ? false : true; //Not used I think
                bone.BillboardMode = b.BillboardMode.ToString();
                //EditData
                //
                //UserData
                addMetaData(ref bone.UserData, b.MetaData);
                //---------
                bone.Transform.Rotate.X = b.Rotation.X;
                bone.Transform.Rotate.Y = b.Rotation.Y;
                bone.Transform.Rotate.Z = b.Rotation.Z;
                bone.Transform.Scale.X = b.Scale.X;
                bone.Transform.Scale.Y = b.Scale.Y;
                bone.Transform.Scale.Z = b.Scale.Z;
                bone.Transform.Translate.X = b.Translation.X;
                bone.Transform.Translate.Y = b.Translation.Y;
                bone.Transform.Translate.Z = b.Translation.Z;
                bones.Add(bone);
            }

            //XML Serializer
            XmlWriterSettings settings = new XmlWriterSettings {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t"
            };
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(typeof(CtrModel));
            XmlWriter output = XmlWriter.Create(new FileStream(fileName, FileMode.Create), settings);
            serializer.Serialize(output, ctrMdl, ns);
            output.Close();
        }

        private static string genBoneIndex(H3DVertex[] verts) {
            StringBuilder sb = new StringBuilder("\n");
            foreach (var v in verts) {
                sb.Append(string.Join(" ", v.Indices) + "\n");
            }
            return sb.ToString();
        }

        private static string genBoneWeights(H3DVertex[] verts) {
            StringBuilder sb = new StringBuilder("\n");
            foreach (var v in verts) {
                for (int w = 0; w < 4; w++)
                    sb.Append(string.Join(" ", Convert.ToByte(v.Weights[w]).ToString(CultureInfo.InvariantCulture) + (w < 3 ? " " : "\n")));
            }
            return sb.ToString();
        }

        private static string genVec2Array(H3DVertex[] verts, PICAAttributeFormat format, PICAAttributeName name) {
            StringBuilder sb = new StringBuilder("\n");
            Vector2D vector = new Vector2D();
            foreach (var vec in verts) {
                switch (name) {
                    case PICAAttributeName.TextureCoordinate0:
                        vector = vec.TextureCoord0;
                        break;
                }
                switch (format) {
                    case PICAAttributeFormat.Byte:
                        sb.Append(
                        ((sbyte)vector.X).ToString(CultureInfo.InvariantCulture) + " " +
                        ((sbyte)vector.Y).ToString(CultureInfo.InvariantCulture) + "\n"
                        );
                        break;
                    case PICAAttributeFormat.Ubyte:
                        sb.Append(
                        ((byte)vector.X).ToString(CultureInfo.InvariantCulture) + " " +
                        ((byte)vector.Y).ToString(CultureInfo.InvariantCulture) + "\n"
                        );
                        break;
                    case PICAAttributeFormat.Short:
                        sb.Append(
                        ((short)vector.X).ToString(CultureInfo.InvariantCulture) + " " +
                        ((short)vector.Y).ToString(CultureInfo.InvariantCulture) + "\n"
                        );
                        break;
                    case PICAAttributeFormat.Float:
                        sb.Append(
                        vector.X.ToString(CultureInfo.InvariantCulture) + " " +
                        vector.Y.ToString(CultureInfo.InvariantCulture) + "\n"
                        );
                        break;
                }
            }
            return sb.ToString();
        }

        private static string genVec3Array(H3DVertex[] verts, PICAAttributeFormat format, PICAAttributeName name) {
            StringBuilder sb = new StringBuilder("\n");
            Vector3D vector = new Vector3D();
            foreach (var vec in verts) {
                switch (name) {
                    case PICAAttributeName.Position: 
                        vector = vec.Position;
                        break;
                    case PICAAttributeName.Normal:
                        vector = vec.Normal;
                        break;
                }
                switch (format) {
                    case PICAAttributeFormat.Byte:
                        sb.Append(
                        ((sbyte)vector.X).ToString(CultureInfo.InvariantCulture) + " " +
                        ((sbyte)vector.Y).ToString(CultureInfo.InvariantCulture) + " " +
                        ((sbyte)vector.Z).ToString(CultureInfo.InvariantCulture) + "\n"
                        );
                        break;
                    case PICAAttributeFormat.Ubyte:
                        sb.Append(
                        ((byte)vector.X).ToString(CultureInfo.InvariantCulture) + " " +
                        ((byte)vector.Y).ToString(CultureInfo.InvariantCulture) + " " +
                        ((byte)vector.Z).ToString(CultureInfo.InvariantCulture) + "\n"
                        );
                        break;
                    case PICAAttributeFormat.Short:
                        sb.Append(
                        ((short)vector.X).ToString(CultureInfo.InvariantCulture) + " " +
                        ((short)vector.Y).ToString(CultureInfo.InvariantCulture) + " " +
                        ((short)vector.Z).ToString(CultureInfo.InvariantCulture) + "\n"
                        );
                        break;
                    case PICAAttributeFormat.Float:
                        sb.Append(
                        vector.X.ToString(CultureInfo.InvariantCulture) + " " +
                        vector.Y.ToString(CultureInfo.InvariantCulture) + " " +
                        vector.Z.ToString(CultureInfo.InvariantCulture) + "\n"
                        );
                        break;
                }
            }
            return sb.ToString();
        }

        private static string getTestFunc(PICATestFunc opt) { //The idea of this function is absurd, but these strings were too bad to add to the enums
            string func = "";
            switch (opt) { 
                case PICATestFunc.LessThan:
                    func = "Less";
                    break;
                case PICATestFunc.LessThanEqu:
                    func = "Lequal";
                    break;
                case PICATestFunc.GreaterThanEqu:
                    func = "Gequal";
                    break;
                case PICATestFunc.GreaterThan:
                    func = "Greater";
                    break;
                default:
                    func = opt.ToString();
                    break;
            }
            return func;
        }

        private static void addDccToolMeta(ref ctrEditData editData, string node) {
            ctrNodeName n;
            if (editData == null) editData = new ctrEditData();
            editData.DccToolSourceNodeMetaData = new ctrDccSrcNode();
            editData.DccToolSourceNodeMetaData.Key = "DccToolSourceNode";
            n = new ctrNodeName();
            n.Name = node;
            editData.DccToolSourceNodeMetaData.Values.Add(n);
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
                    case H3DMetaDataType.UnicodeString: 
                        {
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
                    case H3DMetaDataType.Integer: 
                        {
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
                    case H3DMetaDataType.Single: 
                        {
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
