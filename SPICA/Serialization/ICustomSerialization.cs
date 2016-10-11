namespace SPICA.Serialization
{
    interface ICustomSerialization
    {
        void Deserialize(BinaryDeserializer Deserializer);
        void Serialize(BinarySerializer Serializer);
    }
}
