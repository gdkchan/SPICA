using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SPICA.Formats.CtrH3D
{
    public struct H3DMetaDataValue : ICustomSerialization, INamed
    {
        private string _Name;

        [XmlAttribute]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [XmlAttribute] public H3DMetaDataType Type;

        [Ignore]
        [XmlElement(Type = typeof(int))]
        [XmlElement(Type = typeof(float))]
        [XmlElement(Type = typeof(string))]
        [XmlElement(Type = typeof(H3DBoundingBox))]
        public List<object> Values;

        public object this[int Index]
        {
            get { return Values[Index]; }
            set { Values[Index] = value; }
        }

        public H3DMetaDataValue(string Name, object Value)
        {
            Type ValueType = Value.GetType();

            if (ValueType == typeof(int))
                Type = H3DMetaDataType.Integer;
            else if (ValueType == typeof(float))
                Type = H3DMetaDataType.Single;
            else if (ValueType == typeof(string))
                Type = H3DMetaDataType.ASCIIString;
            else if (ValueType == typeof(H3DBoundingBox))
                Type = H3DMetaDataType.BoundingBox;
            else
                throw new ArgumentException($"Type {ValueType} is not valid as Meta Data!");

            _Name = $"${Name}";
            Values = new List<object> { Value };
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            ushort Count = Deserializer.Reader.ReadUInt16();
            uint Address = Deserializer.Reader.ReadUInt32();

            long Position = Deserializer.BaseStream.Position;

            Deserializer.BaseStream.Seek(Address, SeekOrigin.Begin);

            Values = new List<object>();

            for (int Index = 0; Index < Count; Index++)
            {
                switch (Type)
                {
                    case H3DMetaDataType.Integer: Values.Add(Deserializer.Reader.ReadInt32()); break;
                    case H3DMetaDataType.Single: Values.Add(Deserializer.Reader.ReadSingle()); break;

                    case H3DMetaDataType.ASCIIString:
                        Deserializer.BaseStream.Seek(Address + Index * 4, SeekOrigin.Begin);
                        Deserializer.BaseStream.Seek(Deserializer.Reader.ReadUInt32(), SeekOrigin.Begin);

                        using (MemoryStream MS = new MemoryStream())
                        {
                            byte Chr;
                            while ((Chr = Deserializer.Reader.ReadByte()) != 0)
                            {
                                MS.WriteByte(Chr);
                            }

                            Values.Add(Encoding.ASCII.GetString(MS.ToArray()));
                        }
                        break;
                    case H3DMetaDataType.UnicodeString:
                        Deserializer.BaseStream.Seek(Address + Index * 4, SeekOrigin.Begin);
                        Deserializer.BaseStream.Seek(Deserializer.Reader.ReadUInt32(), SeekOrigin.Begin);

                        using (MemoryStream MS = new MemoryStream())
                        {
                            ushort Chr;
                            while ((Chr = Deserializer.Reader.ReadUInt16()) != 0)
                            {
                                MS.WriteByte((byte)Chr);
                                MS.WriteByte((byte)(Chr >> 8));
                            }

                            Values.Add(Encoding.Unicode.GetString(MS.ToArray()));
                        }
                        break;

                    case H3DMetaDataType.BoundingBox:
                        Deserializer.BaseStream.Seek(Address + Index * 0x3c, SeekOrigin.Begin);

                        Values.Add(Deserializer.Deserialize<H3DBoundingBox>());
                        break;

                    default: throw new NotImplementedException();
                }
            }

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //FIXME: Unicode Strings will serialize as ASCII too
            Serializer.Strings.Values.Add(new RefValue
            {
                Value    = Name,
                Position = Serializer.BaseStream.Position
            });

            Serializer.Contents.Values.Add(new RefValue
            {
                Value    = Values,
                Position = Serializer.BaseStream.Position + 8
            });

            Serializer.Writer.Write(0u);
            Serializer.Writer.Write((ushort)Type);
            Serializer.Writer.Write((ushort)Values.Count);
            Serializer.Writer.Write(0u);

            return true;
        }
    }
}
