using SPICA.Serialization;

using System;
using System.IO;
using System.Text;

namespace SPICA.Formats.H3D
{
    class H3DMetaDataValue : ICustomDeserializer
    {
        public string Name;
        public H3DMetaDataType Type;

        [NonSerialized]
        public object[] Values;

        public object this[int Index]
        {
            get { return Values[Index]; }
            set { Values[Index] = value; }
        }

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            ushort Count = Deserializer.Reader.ReadUInt16();
            uint Address = Deserializer.Reader.ReadUInt32();

            long Position = Deserializer.BaseStream.Position;

            Deserializer.BaseStream.Seek(Address, SeekOrigin.Begin);

            Values = new object[Count];

            for (int Index = 0; Index < Count; Index++)
            {
                switch (Type)
                {
                    case H3DMetaDataType.Integer: Values[Index] = Deserializer.Reader.ReadInt32(); break;
                    case H3DMetaDataType.Single: Values[Index] = Deserializer.Reader.ReadSingle(); break;

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

                            Values[Index] = Encoding.ASCII.GetString(MS.ToArray());
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

                            Values[Index] = Encoding.Unicode.GetString(MS.ToArray());
                        }
                        break;

                    case H3DMetaDataType.BoundingBox:
                        Deserializer.BaseStream.Seek(Address + Index * 0x3c, SeekOrigin.Begin);

                        Values[Index] = Deserializer.Deserialize<H3DBoundingBox>();
                        break;
                }
            }

            Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
        }
    }
}
