using System;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEAsset
    {
        public DateTime created;
        public DateTime modified;
        public string up_axis = "Y_UP";

        public DAEAsset()
        {
            created = DateTime.Now;
            modified = DateTime.Now;
        }
    }
}
