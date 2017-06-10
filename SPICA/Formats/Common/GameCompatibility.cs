using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model.Material;

namespace SPICA.Formats.Common
{
    static class GameCompatibility
    {
        public static void AddMetaDataForCompat(H3DMaterial Material)
        {
            H3DMaterialParams Params = Material.MaterialParams;

            /*
             * Some games expects to find specific data stored as Meta Data on binary model files,
             * when this data isn't found the game will most likely crash because it doesn't know
             * how to properly load the model without it. This method adds Meta Data found on the
             * most common games to ensure that they will be loaded correctly.
             */
            Params.MetaData = new H3DMetaData();

            Params.MetaData.Values.Add(new H3DMetaDataValue("EdgeType", 0));
            Params.MetaData.Values.Add(new H3DMetaDataValue("IDEdgeEnable", 0));
            Params.MetaData.Values.Add(new H3DMetaDataValue("EdgeID", 100));
            Params.MetaData.Values.Add(new H3DMetaDataValue("ProjectionType", 0));
            Params.MetaData.Values.Add(new H3DMetaDataValue("RimPow", 8f));
            Params.MetaData.Values.Add(new H3DMetaDataValue("RimScale", 1f));
            Params.MetaData.Values.Add(new H3DMetaDataValue("PhongPow", 8f));
            Params.MetaData.Values.Add(new H3DMetaDataValue("PhongScale", 1f));
            Params.MetaData.Values.Add(new H3DMetaDataValue("IDEdgeOffsetEnable", 0));
            Params.MetaData.Values.Add(new H3DMetaDataValue("EdgeMapAlphaMask", 0));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeTexture0", 0));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeTexture1", 0));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeTexture2", 0));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeConstant0", Params.Constant0Assignment));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeConstant1", Params.Constant1Assignment));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeConstant2", Params.Constant2Assignment));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeConstant3", Params.Constant3Assignment));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeConstant4", Params.Constant4Assignment));
            Params.MetaData.Values.Add(new H3DMetaDataValue("BakeConstant5", Params.Constant5Assignment));
            Params.MetaData.Values.Add(new H3DMetaDataValue("VertexShaderType", 16384));
            Params.MetaData.Values.Add(new H3DMetaDataValue("ShaderParam0", 1f));
            Params.MetaData.Values.Add(new H3DMetaDataValue("ShaderParam1", 1f));
            Params.MetaData.Values.Add(new H3DMetaDataValue("ShaderParam2", 1f));
            Params.MetaData.Values.Add(new H3DMetaDataValue("ShaderParam3", 1f));
            Params.MetaData.Values.Add(new H3DMetaDataValue("PrivateMod", 65, 674376750));
        }
    }
}
