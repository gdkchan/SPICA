namespace SPICA.Formats.CtrH3D
{
    public class H3DMetaData
    {
        public readonly H3DDict<H3DMetaDataValue> Values;

        public H3DMetaData()
        {
            Values = new H3DDict<H3DMetaDataValue>();
        }
    }
}
