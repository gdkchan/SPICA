namespace SPICA.Serialization
{
    interface ICustomDeserializer
    {
        void Deserialize(BinaryDeserializer Deserializer, string FName);
    }
}
