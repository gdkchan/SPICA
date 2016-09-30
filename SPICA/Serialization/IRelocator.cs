namespace SPICA.Serialization
{
    interface IRelocator
    {
        void AddPointer(long Position, string Hint = null);
        void AddSection(long Position, long Length, string Name);
    }
}
