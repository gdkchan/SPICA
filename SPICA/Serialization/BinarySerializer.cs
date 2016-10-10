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

        public struct ReferenceValue
        {
            public FieldInfo Info;
            public object Value;
            public long Position;
            public bool HasLength;
        }

        private class ObjPointer
        {
            public uint Position;
            public int Length;
        }

        public List<ReferenceValue> Contents;

        public Queue<ReferenceValue> Strings;
        public Queue<ReferenceValue> Commands;
        public Queue<ReferenceValue> RawData;

        private Dictionary<object, ObjPointer> ObjPointers;

        private bool HasBuffered = false;
        private uint BufferedUInt = 0;
        private uint BufferedShift = 0;

        public BinarySerializer(Stream Stream)
        {
            BaseStream = Stream;
            Writer = new BinaryWriter(Stream);

            Contents = new List<ReferenceValue>();

            Strings = new Queue<ReferenceValue>();
            Commands = new Queue<ReferenceValue>();
            RawData = new Queue<ReferenceValue>();

            ObjPointers = new Dictionary<object, ObjPointer>();
        }

        public void Serialize(object Value)
        {
            WriteValue(Value);

            WriteSection(Strings, 0x10);
            WriteSection(Commands, 0x80);
            WriteSection(RawData, 0x80);
        }

        private void WriteSection(Queue<ReferenceValue> Section, int Align)
        {
            while (Section.Count > 0) WriteValue(Section.Dequeue());
            while ((BaseStream.Position % Align) != 0) BaseStream.WriteByte(0);
        }

        private void WriteValue(object Value, FieldInfo Info = null, bool IsElem = false)
        {
            Type Type = Value.GetType();
            long Position = BaseStream.Position;

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
                WriteObject(Value, IsElem);
            }

            //Add a reference to this Object and its position to avoid writing it more than once
            if (!(ObjPointers.ContainsKey(Value) || Type.IsPrimitive || Type.IsEnum))
            {
                ObjPointers.Add(Value, new ObjPointer
                {
                    Position = (uint)Position,
                    Length = (int)(BaseStream.Position - Position)
                });
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

            foreach (object Value in List)
            {
                if (Pointers)
                {
                    Contents.Add(new ReferenceValue
                    {
                        Info = null,
                        Value = Value,
                        Position = BaseStream.Position,
                        HasLength = false
                    });

                    Skip(4);
                }
                else
                {
                    WriteValue(Value, null, true);
                }
            }
        }

        private void WriteValue(ReferenceValue Reference)
        {
            object Value = Reference.Value;

            if (Value != null)
            {
                FieldInfo Info = Reference.Info;
                ObjPointer OPtr = GetObjPointer(Value, Info);
                long Position = BaseStream.Position;
                bool Range = Info != null && Info.IsDefined(typeof(RangeAttribute));

                BaseStream.Seek(Reference.Position, SeekOrigin.Begin);

                Writer.Write(OPtr.Position);

                if (Reference.HasLength && !Range)
                {
                    Writer.Write(((IList)Value).Count);
                }

                BaseStream.Seek(Position, SeekOrigin.Begin);

                if (OPtr.Position == Position) WriteValue(Value, Info);

                if (Range)
                {
                    Position = BaseStream.Position;

                    BaseStream.Seek(Reference.Position + 4, SeekOrigin.Begin);

                    Writer.Write((uint)(OPtr.Length != 0 ? OPtr.Length : Position));

                    BaseStream.Seek(Position, SeekOrigin.Begin);
                }
            }
        }

        private ObjPointer GetObjPointer(object Value, FieldInfo Info)
        {
            ObjPointer Output = new ObjPointer
            {
                Position = (uint)BaseStream.Position,
                Length = 0
            };

            if (ObjPointers.ContainsKey(Value))
            {
                Output = ObjPointers[Value];
            }
            else if (Value is IList)
            {
                uint SPos = 0;
                int EPos = 0;
                int Matches = 0;

                foreach (object Elem in ((IList)Value))
                {
                    if (ObjPointers.ContainsKey(Elem) && (ObjPointers[Elem].Position == EPos || EPos == 0))
                    {
                        if (Matches++ == 0) EPos = (int)(SPos = ObjPointers[Elem].Position);

                        EPos += ObjPointers[Elem].Length;
                    }
                    else
                    {
                        break;
                    }
                }

                if (Matches > 0 && Matches == ((IList)Value).Count)
                {
                    Output.Position = SPos;
                    Output.Length = EPos;
                }
            }

            return Output;
        }

        public void WriteObject(object Value, bool IsElem = false)
        {
            Type ValueType = Value.GetType();

            int Index = Contents.Count;

            if (Value is ICustomSerializer)
            {
                ((ICustomSerializer)Value).Serialize(this);
            }

            foreach (FieldInfo Info in ValueType.GetFields())
            {
                if (!Info.IsDefined(typeof(NonSerializedAttribute)))
                {
                    Type Type = Info.FieldType;

                    bool Inline;

                    Inline = Info.IsDefined(typeof(InlineAttribute));
                    Inline |= Type.IsDefined(typeof(InlineAttribute));

                    if (Type.IsValueType || Type.IsEnum || Inline)
                    {
                        WriteValue(Info.GetValue(Value), Info);
                    }
                    else
                    {
                        bool IsList = typeof(IList).IsAssignableFrom(Type);
                        bool HasLength = !Info.IsDefined(typeof(FixedLengthAttribute)) && IsList;

                        Enqueue(new ReferenceValue
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

            if (!IsElem && (ValueType.IsClass && !ValueType.IsDefined(typeof(InlineAttribute))))
            {
                while (Index < Contents.Count)
                {
                    WriteValue(Contents[Index]);
                    Contents.RemoveAt(Index);
                }
            }

            if (HasBuffered) WriteBool();
        }

        private void Enqueue(ReferenceValue Value)
        {
            Type Type = Value.Info.FieldType;

            if (Type == typeof(string))
            {
                Strings.Enqueue(Value);
            }
            else if (Type == typeof(uint[]))
            {
                Commands.Enqueue(Value);
            }
            else
            {
                Contents.Add(Value);
            }
        }

        public void Skip(int Bytes)
        {
            while (Bytes-- > 0) BaseStream.WriteByte(0);
        }
    }
}
