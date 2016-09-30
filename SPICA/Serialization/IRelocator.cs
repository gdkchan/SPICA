namespace SPICA.Serialization
{
    interface IRelocator
    {
        void AddPointer(long Position, int Section = -1);
        void AddSection(long Position, long Length, string Name);
    }
}
