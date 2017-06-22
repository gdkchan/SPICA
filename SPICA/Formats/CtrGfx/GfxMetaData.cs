using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D;
using SPICA.Math3D;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace SPICA.Formats.CtrGfx
{
    public enum GfxStringFormat : uint
    {
        ASCII,
        UTF8,
        UTF16LE,
        UTF16BE
    }

    public class GfxMetaData : ICustomSerialization, IEnumerable<object>, INamed
    {
        private uint InheritedType;

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value ?? throw Exceptions.GetNullException("Name");
            }
        }

        public GfxMetaDataType Type;

        [Ignore] public GfxStringFormat StringFormat;

        [Ignore] private List<object> Values;

        public int Count { get { return Values.Count; } }

        public object this[int Index]
        {
            get
            {
                return Values[Index];
            }
            set
            {
                Values[Index] = value;
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            uint Count = Deserializer.Reader.ReadUInt32();
            uint Type2 = Deserializer.Reader.ReadUInt32();

            if (Type == GfxMetaDataType.String)
            {
                StringFormat = (GfxStringFormat)Deserializer.Reader.ReadUInt32();
            }

            Values = new List<object>();

            for (int Index = 0; Index < Count; Index++)
            {
                switch (Type)
                {
                    case GfxMetaDataType.Single:  Values.Add(Deserializer.Reader.ReadSingle());  break;
                    case GfxMetaDataType.Integer: Values.Add(Deserializer.Reader.ReadInt32());   break;
                    case GfxMetaDataType.String:  Values.Add(ReadString(Deserializer));          break;
                    case GfxMetaDataType.Vector3: Values.Add(Deserializer.Reader.ReadVector3()); break;
                    case GfxMetaDataType.Color:   Values.Add(Deserializer.Reader.ReadVector4()); break;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Sections[(uint)H3DSectionId.Strings].Values.Add(new RefValue
            {
                Value    = _Name,
                Position = Serializer.BaseStream.Position
            });

            Serializer.Writer.Write(0u);
            Serializer.Writer.Write((uint)Type);
            Serializer.Writer.Write(Values.Count);

            if (Type == GfxMetaDataType.String)
            {
                Serializer.Writer.Write((uint)StringFormat);
            }

            foreach (object Value in Values)
            {
                switch (Type)
                {
                    case GfxMetaDataType.Single:  Serializer.Writer.Write((float)Value);   break;
                    case GfxMetaDataType.Integer: Serializer.Writer.Write((int)Value);     break;
                    case GfxMetaDataType.String:  WriteString(Serializer, (string)Value);  break;
                    case GfxMetaDataType.Vector3: Serializer.Writer.Write((Vector3)Value); break;
                    case GfxMetaDataType.Color:   Serializer.Writer.Write((Vector4)Value); break;
                }
            }

            return true;
        }

        private string ReadString(BinaryDeserializer Deserializer)
        {
            long Position = Deserializer.BaseStream.Position + 4;

            Deserializer.BaseStream.Seek(Deserializer.ReadPointer(), SeekOrigin.Begin);

            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                while (true)
                {
                    if (StringFormat == GfxStringFormat.UTF16LE ||
                        StringFormat == GfxStringFormat.UTF16BE)
                    {
                        ushort Value = Deserializer.Reader.ReadUInt16();

                        if (Value == 0)
                        {
                            break;
                        }
                        else if (StringFormat == GfxStringFormat.UTF16BE)
                        {
                            Value = (ushort)((Value << 8) | (Value >> 8));
                        }

                        Writer.Write(Value);
                    }
                    else
                    {
                        byte Value = Deserializer.Reader.ReadByte();

                        if (Value == 0) break;

                        Writer.Write(Value);
                    }
                }

                byte[] Buffer = MS.ToArray();

                Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);

                switch (StringFormat)
                {
                    case GfxStringFormat.ASCII:   return Encoding.ASCII.GetString(Buffer);
                    case GfxStringFormat.UTF8:    return Encoding.UTF8.GetString(Buffer);
                    case GfxStringFormat.UTF16LE: return Encoding.Unicode.GetString(Buffer);
                    case GfxStringFormat.UTF16BE: return Encoding.Unicode.GetString(Buffer);
                }
            }

            return null;
        }

        private void WriteString(BinarySerializer Serializer, string Value)
        {
            //FIXME: This only works for ASCII strings, support unicode too!
            Serializer.Sections[(uint)H3DSectionId.Strings].Values.Add(new RefValue
            {
                Value    = Value,
                Position = Serializer.BaseStream.Position
            });

            Serializer.BaseStream.Seek(4, SeekOrigin.Current);
        }
    }
}
