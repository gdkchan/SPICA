using System;

namespace SPICA.Formats.Generic.COLLADA
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
