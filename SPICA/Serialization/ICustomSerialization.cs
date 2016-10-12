namespace SPICA.Serialization
{
    interface ICustomSerialization
    {
        void Deserialize(BinaryDeserializer Deserializer);
        bool Serialize(BinarySerializer Serializer);
    }
}
