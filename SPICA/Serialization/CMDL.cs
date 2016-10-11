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
        public class ctrUserData {
            [XmlElement("IntegerArrayMetaDataXml")]
            public List<ctrIntArrayMeta> intUserData;

            [XmlElement("FloatArrayMetaDataXml")]
            public List<ctrFloatArrayMeta> floatUserData;

            [XmlElement("StringArrayMetaDataXml")]
            public List<ctrStringArrayMeta> strUserData;
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

            public ctrUserData UserData = new ctrUserData();

            public ctrEditData EditData = null;

            [XmlArrayItem("GraphicsAnimationGroupDescription")]
            public List<ctrGraphicsAnimGroupDesc> AnimationGroupDescription = new List<ctrGraphicsAnimGroupDesc>();

            public ctrTransform Transform = new ctrTransform();

            [XmlArrayItem("SeperateDataShapeCtr")]
            public List<ctrSeperateDataShape> Shapes = new List<ctrSeperateDataShape>();

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
            public ctrRotation Rotation = new ctrRotation();
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

        public class ctrRotation {
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
        public class ctrSeperateDataShape {
            public ctrOBB OrientedBoundingBox = null;

            public ctrPositionOff PositionOffset = new ctrPositionOff();

            [XmlArrayItem("PrimitiveSetCtr")]
            public List<ctrPrimSet> PrimitiveSets = new List<ctrPrimSet>();

            [XmlArrayItem("Vector3VertexStreamCtr")]
            public List<ctrVertAttrib> VertexAttributes = new List<ctrVertAttrib>();
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
            [XmlArrayItem("UshortIndexStreamCtr")]
            public List<ctrIndexStream> IndexStreams = new List<ctrIndexStream>();
        }

        public class ctrIndexStream {
            [XmlAttribute]
            public string PrimitiveMode;

            [XmlAttribute]
            public string Size;
        }

        public class ctrVertAttrib {
            [XmlAttribute]
            public string Usage;

            [XmlAttribute]
            public int VertexSize;

            [XmlAttribute]
            public float Scale;

            [XmlAttribute]
            public string QuantizedMode;

            [XmlText]
            public string Vec3Array;
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

            public ctrUserData UserData = new ctrUserData();

            public ctrEditData EditData = null;

            public ctrShaderRef ShaderReference = new ctrShaderRef();

            public ctrMatColor MaterialColor = new ctrMatColor();

            public ctrRaster Rasterization = new ctrRaster();

            [XmlArrayItem("TextureCoordinatorCtr")]
            public List<ctrTexCoordinator> TextureCoordinators = new List<ctrTexCoordinator>();

            [XmlArrayItem("PixelBasedTextureMapperCtr")]
            public List<ctrPixTexMap> TextureMappers = new List<ctrPixTexMap>();

            public ctrFragShader FragmentShader = new ctrFragShader();

            public ctrFragOp FragmentOperation = new ctrFragOp();
        }

        public class ctrShaderRef {
            //TODO
        }

        public class ctrMatColor {
            [XmlAttribute]
            public int VertexColorScale;

            public ctrEmission Emission = new ctrEmission();

            public ctrAmbient Ambient = new ctrAmbient();

            public ctrDiffuse Diffuse = new ctrDiffuse();

            public ctrSpec0 Specular0 = new ctrSpec0();

            public ctrSpec1 Specular1 = new ctrSpec1();

            public ctrConst0 Constant0 = new ctrConst0();

            public ctrConst1 Constant1 = new ctrConst1();

            public ctrConst2 Constant2 = new ctrConst2();

            public ctrConst3 Constant3 = new ctrConst3();

            public ctrConst4 Constant4 = new ctrConst4();

            public ctrConst5 Constant5 = new ctrConst5();
        }

        public class ctrEmission {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrAmbient {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrDiffuse {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrSpec0 {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrSpec1 {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrConst0 {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrConst1 {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrConst2 {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrConst3 {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrConst4 {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
        }

        public class ctrConst5 {
            [XmlAttribute]
            public byte R;

            [XmlAttribute]
            public byte G;

            [XmlAttribute]
            public byte B;

            [XmlAttribute]
            public byte A;
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
            public string UvCoordinateMap;

            [XmlAttribute]
            public int ReferenceCamera;

            [XmlAttribute]
            public string MatrixMode;

            [XmlAttribute]
            public int ScaleS;

            [XmlAttribute]
            public int ScaleT;

            [XmlAttribute]
            public int Rotate;

            [XmlAttribute]
            public int TranslateS;

            [XmlAttribute]
            public int TranslateT;
        }

        public class ctrPixTexMap {
            //TODO
        }

        public class ctrFragShader {
            [XmlAttribute]
            public string LayerConfig;
            //TODO
        }

        public class ctrFragOp {
            //TODO
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
                indicies = 0,
                verts = 0;
            foreach (var m in mdl.Meshes) {
                verts += m.GetVertices().Count();
                
                foreach (var s in m.SubMeshes) {
                    subMeshes++;
                    indicies += s.Indices.Count();
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
            note.Value = (uint)indicies / 3;
            notes.Add(note);
            note = new ctrNote();
            note.Name = "TotalVertexCount";
            note.Value = (uint)verts;
            notes.Add(note);

            //Model data
            ctrSkeletalModel skelModel = ctrMdl.GraphicsContentCtr.Models.SkeletalModel;
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
            ctrUserData modelUserData = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.UserData;
            getMetaData(modelUserData, mdl.MetaData);

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
            optLogArr.Values.Add(optLog);

            //Skeleton Animation
            List<ctrGraphicsAnimGroupDesc> animGroups = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.AnimationGroupDescription;
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
            trans.Rotation.X = 0;
            trans.Rotation.Y = 0;
            trans.Rotation.Z = 0;
            trans.Translate.X = 0;
            trans.Translate.Y = 0;
            trans.Translate.Z = 0;
            
            //Shapes
            List<ctrSeperateDataShape> shapes = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.Shapes;
            ctrSeperateDataShape shape;
            ctrOBB obb;
            ctrPrimSet primSet = null;
            ctrVertAttrib vertAtt;
            StringBuilder sb;
            foreach (var sh in mdl.Meshes) {
                shape = new ctrSeperateDataShape();
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
                shape.PositionOffset.X = 0;
                shape.PositionOffset.Y = 0;
                shape.PositionOffset.Z = 0;
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
                    var boneInd = new List<ushort>(sub.BoneIndices);
                    for (int bi = sub.BoneIndices.Count() - 1; bi > 0; bi--) {
                        if (boneInd[bi] == 0) boneInd.RemoveAt(bi); //Remove 0s at the end of the array
                        else break;
                    }
                    primSet.BoneIndexTable = string.Join(" ", boneInd);
                }
                foreach (var att in sh.Attributes) {
                    string quantized = "";
                    switch (att.Format) {
                        case PICAAttributeFormat.SignedByte:
                            quantized = "Byte";
                            break;
                        case PICAAttributeFormat.SignedShort:
                            quantized = "Short";
                            break;
                        case PICAAttributeFormat.Single:
                            quantized = "Float";
                            break;
                        case PICAAttributeFormat.UnsignedByte:
                            quantized = "Ubyte";
                            break;
                    }
                    vertAtt = new ctrVertAttrib();
                    vertAtt.Usage = att.Name.ToString();
                    vertAtt.VertexSize = verts;
                    vertAtt.Scale = att.Scale;
                    vertAtt.QuantizedMode = quantized;
                    sb = new StringBuilder("\n");
                    foreach (var vec in sh.GetVertices()) {
                        switch (att.Name) {
                            case PICAAttributeName.Position:
                                sb.Append(
                                    vec.Position.X + " " +
                                    vec.Position.Y + " " +
                                    vec.Position.Z + "\n"
                                    );
                                break;
                            case PICAAttributeName.Normal:
                                sb.Append(
                                    vec.Normal.X + " " +
                                    vec.Normal.Y + " " +
                                    vec.Normal.Z + "\n"
                                    );
                                break;
                            case PICAAttributeName.BoneIndex:
                                sb.Append(string.Join(" ", vec.Indices) + "\n");
                                break;
                            case PICAAttributeName.BoneWeight:
                                sb.Append(string.Join(" ", vec.Weights) + "\n");
                                break;
                            case PICAAttributeName.Color: //TODO Determine if this is correct format
                                sb.Append(
                                    vec.Color.R + " " +
                                    vec.Color.G + " " +
                                    vec.Color.B + " " +
                                    vec.Color.A + "\n"
                                    );
                                break;
                            case PICAAttributeName.Tangent:
                                sb.Append(
                                    vec.Tangent.X + " " +
                                    vec.Tangent.Y + " " +
                                    vec.Tangent.Z + "\n"
                                    );
                                break;
                            case PICAAttributeName.TextureCoordinate0:
                                sb.Append(
                                    vec.TextureCoord0.X + " " +
                                    vec.TextureCoord0.Y + "\n"
                                    );
                                break;
                            case PICAAttributeName.TextureCoordinate1:
                                sb.Append(
                                    vec.TextureCoord1.X + " " +
                                    vec.TextureCoord1.Y + "\n"
                                    );
                                break;
                            case PICAAttributeName.TextureCoordinate2:
                                sb.Append(
                                    vec.TextureCoord2.X + " " +
                                    vec.TextureCoord2.Y + "\n"
                                    );
                                break;
                        }
                    }
                    vertAtt.Vec3Array = sb.ToString();
                    shape.VertexAttributes.Add(vertAtt);
                }
                shape.PrimitiveSets.Add(primSet);
                shapes.Add(shape);
            }

            //Materials
            List<ctrMaterial> mats = ctrMdl.GraphicsContentCtr.Models.SkeletalModel.Materials;
            ctrMaterial mat;
            ctrShaderRef shadeRef;
            ctrMatColor matCol;
            ctrRaster raster;
            List<ctrTexCoordinator> texCoords;
            List<ctrPixTexMap> texMaps;
            ctrFragShader fragShade;
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
                getMetaData(mat.UserData, mt.MaterialParams.MetaData);
                //ShaderReference
                //TODO
                //MaterialColor
                matCol = new ctrMatColor();
                matCol.VertexColorScale = 1; //TODO: is this used?
                mat.MaterialColor = matCol;
                //Rasterization
                raster = new ctrRaster();
                raster.CullingMode = mt.MaterialParams.FaceCulling.ToString();
                raster.IsPolygonOffsetEnabled = mt.MaterialParams.PolygonOffsetUnit > 0;
                raster.PolygonOffsetUnit = mt.MaterialParams.PolygonOffsetUnit;
                mat.Rasterization = raster;
                //TextureCoordinators
                //TODO
                //TextureMappers
                //TODO
                //FragmentShader
                fragShade = new ctrFragShader();
                fragShade.LayerConfig = "ConfigurationType" + mt.MaterialParams.LayerConfig.ToString();
                mat.FragmentShader = fragShade;
                //FragmentOperations
                //TODO

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
                mesh.MaterialReference = "Materials[\"" + mdl.MeshesTree.Nodes[mdl.Meshes[i].NodeId+1].Name + "\"]@file:" + mdl.Materials[i].Texture0Name + ".ctex";
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
                bone.Transform.Rotation.X = b.Rotation.X;
                bone.Transform.Rotation.Y = b.Rotation.Y;
                bone.Transform.Rotation.Z = b.Rotation.Z;
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

        private static void getMetaData(ctrUserData userData, H3DMetaData metaData) {
            ctrIntArrayMeta intArr;
            ctrFloatArrayMeta floatArr;
            ctrStringArrayMeta strArr;
            ctrFloatSet fs;
            ctrIntSet ins;
            ctrStringSet strs;
            userData.intUserData = new List<ctrIntArrayMeta>();
            userData.floatUserData = new List<ctrFloatArrayMeta>();
            userData.strUserData = new List<ctrStringArrayMeta>();
            foreach (var val in metaData?.Values) {
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
                            userData.strUserData.Add(strArr);
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
                            userData.intUserData.Add(intArr);
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
                            userData.floatUserData.Add(floatArr);
                            break;
                        }
                }
            }
            if (userData.floatUserData.Count == 0) userData.floatUserData = null; //These 3 lines are to make sure there are no stray tags
            if (userData.intUserData.Count == 0) userData.intUserData = null;
            if (userData.strUserData.Count == 0) userData.strUserData = null;
        }
    }
}
