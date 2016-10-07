using SPICA.Serialization.Attributes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SPICA.Serialization
{
    class BinarySerializer
    {
        public Stream BaseStream;
        public BinaryWriter Writer;

        private struct ReferenceValue
        {
            public FieldInfo Info;
            public object Value;
            public long Position;
            public bool HasLength;
        }

        private Queue<ReferenceValue> ReferencedValues;

        private bool HasBuffered = false;
        private uint BufferedUInt = 0;
        private uint BufferedShift = 0;

        public BinarySerializer(Stream Stream)
        {
            BaseStream = Stream;
            Writer = new BinaryWriter(Stream);

            ReferencedValues = new Queue<ReferenceValue>();
        }

        public void Serialize(object Value)
        {
            WriteValue(Value);

            while (ReferencedValues.Count > 0) WriteValue(ReferencedValues.Dequeue());

            if (HasBuffered) WriteBool();
        }

        private void WriteValue(object Value, FieldInfo Info = null)
        {
            Type Type = Value.GetType();

            if (HasBuffered && !(Value is bool)) WriteBool();

            if (Type.IsPrimitive || Type.IsEnum)
            {
                switch (Type.GetTypeCode(Type))
                {
                    case TypeCode.UInt64: Writer.Write((ulong)Value); break;
                    case TypeCode.UInt32: Writer.Write((uint)Value); break;
                    case TypeCode.UInt16: Writer.Write((ushort)Value); break;
                    case TypeCode.Byte: Writer.Write((byte)Value); break;
                    case TypeCode.Int64: Writer.Write((long)Value); break;
                    case TypeCode.Int32: Writer.Write((int)Value); break;
                    case TypeCode.Int16: Writer.Write((short)Value); break;
                    case TypeCode.SByte: Writer.Write((sbyte)Value); break;
                    case TypeCode.Single: Writer.Write((float)Value); break;
                    case TypeCode.Double: Writer.Write((double)Value); break;
                    case TypeCode.Boolean:
                        HasBuffered = true;
                        BufferedUInt <<= 1;
                        BufferedUInt |= (uint)(((bool)Value) ? 1 : 0);

                        if (++BufferedShift == 32) WriteBool();

                        break;
                }
            }
            else if (Value is byte[])
            {
                Writer.Write((byte[])Value);
            }
            else if (Value is IList)
            {
                WriteList((IList)Value, Info);
            }
            else if (Value is string)
            {
                Writer.Write(Encoding.ASCII.GetBytes((string)Value + '\0'));
            }
            else
            {
                WriteObject(Value);
            }
        }

        private void WriteBool()
        {
            Writer.Write(BufferedUInt);

            BufferedShift = 0;
            HasBuffered = false;
        }

        private void WriteList(IList List, FieldInfo Info)
        {
            bool Pointers = Info != null && Info.IsDefined(typeof(PointersAttribute));

            long PointersPosition = BaseStream.Position;

            if (Pointers) Skip(List.Count * 4);

            for (int Index = 0; Index < List.Count; Index++)
            {
                if (Pointers)
                {
                    long Position = BaseStream.Position;

                    BaseStream.Seek(PointersPosition + Index * 4, SeekOrigin.Begin);

                    Writer.Write((uint)Position);

                    BaseStream.Seek(Position, SeekOrigin.Begin);
                }

                WriteValue(List[Index]);
            }
        }

        private void WriteValue(ReferenceValue Reference)
        {
            long Position = BaseStream.Position;
            object Value = Reference.Value;

            if (Value != null)
            {
                bool Range = Reference.Info.IsDefined(typeof(RangeAttribute));

                BaseStream.Seek(Reference.Position, SeekOrigin.Begin);

                Writer.Write((uint)Position);

                if (Reference.HasLength && !Range)
                {
                    Writer.Write(((IList)Value).Count);
                }

                BaseStream.Seek(Position, SeekOrigin.Begin);

                WriteValue(Value);

                if (Range)
                {
                    Position = BaseStream.Position;

                    BaseStream.Seek(Reference.Position + 4, SeekOrigin.Begin);

                    Writer.Write((uint)Position);

                    BaseStream.Seek(Position, SeekOrigin.Begin);
                }
            }
        }

        private void WriteObject(object Value)
        {
            foreach (FieldInfo Info in Value.GetType().GetFields())
            {
                if (!Info.IsDefined(typeof(NonSerializedAttribute)))
                {
                    Type Type = Info.FieldType;

                    bool Inline;

                    Inline = Info.IsDefined(typeof(InlineAttribute));
                    Inline |= Type.IsDefined(typeof(InlineAttribute));

                    if (Type.IsValueType || Type.IsEnum || Inline)
                    {
                        WriteValue(Info.GetValue(Value));
                    }
                    else
                    {
                        bool IsList = typeof(IList).IsAssignableFrom(Type);
                        bool HasLength = !Info.IsDefined(typeof(FixedLengthAttribute)) && IsList;

                        ReferencedValues.Enqueue(new ReferenceValue
                        {
                            Info = Info,
                            Value = Info.GetValue(Value),
                            Position = BaseStream.Position,
                            HasLength = HasLength
                        });

                        Skip(HasLength ? 8 : 4);
                    }
                }
            }
        }

        private void Skip(int Bytes)
        {
            while (Bytes-- > 0) BaseStream.WriteByte(0);
        }
    }
}
