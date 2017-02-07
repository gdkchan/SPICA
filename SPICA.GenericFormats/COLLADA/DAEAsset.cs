using System;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAEAsset
    {
        public DateTime created;
        public DateTime modified;

        public DAEAsset()
        {
            created = DateTime.Now;
            modified = DateTime.Now;
        }
    }
}
