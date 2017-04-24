namespace SPICA.Formats.CtrH3D
{
    public class H3DMetaData
    {
        public readonly PatriciaList<H3DMetaDataValue> Values;

        public H3DMetaData()
        {
            Values = new PatriciaList<H3DMetaDataValue>();
        }
    }
}
