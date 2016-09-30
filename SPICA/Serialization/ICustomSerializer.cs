namespace SPICA.Serialization
{
    interface ICustomSerializer
    {
        object Serialize(BinarySerializer Serializer, string FName);
    }
}
