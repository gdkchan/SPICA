namespace SPICA.Formats.CtrGfx
{
    class GfxHeader
    {
        public uint   MagicNumber;
        public ushort ByteOrderMark;
        public ushort HeaderLength;
        public uint   Revision;
        public uint   FileLength;
        public uint   UsedSectionsCount;
    }
}
