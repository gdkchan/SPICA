namespace SPICA.Serialization
{
    interface ICustomSerializeCmd
    {
        void SerializeCmd(BinarySerializer Serializer, object Value);
    }
}
