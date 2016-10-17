using SPICA.Formats.H3D;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

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
        public H3DRelocator Relocator;

        public BinaryWriter Writer;

        public int PhysicalAddressCount { get; private set; }

        public struct ObjectInfo
        {
            public uint Position;
            public int Length;

            public void SetEnd(long Position)
            {
                Length = (int)(Position - this.Position);
            }
        }

        public class Section
        {
            public List<RefValue> Values;
            public ObjectInfo Info;

            public Section()
            {
                Values = new List<RefValue>();
            }
        }

        public Section Contents;

        public Section Strings;
        public Section Commands;

        public Section RawDataTex;
        public Section RawDataVtx;
        public Section RawExtTex;
        public Section RawExtVtx;

        public Dictionary<object, ObjectInfo> ObjPointers;

        public List<long> Pointers;

        private bool HasBuffered = false;
        private uint BufferedUInt = 0;
        private uint BufferedShift = 0;

        private const BindingFlags Binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public BinarySerializer(Stream BaseStream, H3DRelocator Relocator = null)
        {
            this.BaseStream = BaseStream;
            this.Relocator = Relocator;

            Writer = new BinaryWriter(BaseStream);

            Contents = new Section();

            Strings = new Section();
            Commands = new Section();

            RawDataTex = new Section();
            RawDataVtx = new Section();
            RawExtTex = new Section();
            RawExtVtx = new Section();

            ObjPointers = new Dictionary<object, ObjectInfo>();

            Pointers = new List<long>();
        }

        public void Serialize(object Value)
        {
            Contents.Info.Position = (uint)BaseStream.Position;

            WriteValue(Value);

            Contents.Info.SetEnd(BaseStream.Position);

            Strings.Values.RemoveAll(x => x.Value == null);
            Strings.Values.Sort(StringUtils.CompareString);

            WriteSection(Strings, 0x10);
            WriteSection(Commands, 0x80);

            PhysicalAddressCount = RawDataTex.Values.Count + RawDataVtx.Values.Count;
            PhysicalAddressCount += RawExtTex.Values.Count + RawExtVtx.Values.Count;

            WriteSection(RawDataTex, 0x80);
            WriteSection(RawDataVtx, 0x80);
            WriteSection(RawExtTex, 0x80);
            WriteSection(RawExtVtx, 0x80);
        }

        private void WriteSection(Section Section, int Align)
        {
            Section.Info.Position = (uint)BaseStream.Position;

            while (Section.Values.Count > 0)
            {
                WriteValue(Section.Values[0]);
                Section.Values.RemoveAt(0);
            }

            Section.Info.SetEnd(BaseStream.Position);

            while ((BaseStream.Position % Align) != 0) BaseStream.WriteByte(0);
        }

        public void WriteValue(object Value, bool IsElem = false)
        {
            Type Type = Value.GetType();
            long Position = BaseStream.Position;

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

                        if (++BufferedShift == 32 || !IsElem) WriteBool();

                        break;
                }

                return;
            }
            else if (Value is IList)
            {
                WriteList((IList)Value);
            }
            else if (Value is string)
            {
                Writer.Write(Encoding.ASCII.GetBytes((string)Value + '\0'));
            }
            else
            {
                WriteObject(Value);
            }

            //Avoid writing the same Object more than once
            if (Type.IsClass) AddObjPointer(Value, Position);
        }

        private void AddObjPointer(object Value, long Position)
        {
            if (!ObjPointers.ContainsKey(Value))
            {
                ObjPointers.Add(Value, new ObjectInfo
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

        private void WriteList(IList List)
        {
            Type Type = List.GetType();

            if (Type.IsArray)
                Type = Type.GetElementType();
            else
                Type = Type.GetGenericArguments()[0];

            bool Inline = Type.IsDefined(typeof(InlineAttribute));
            bool Pointers = !(Type.IsValueType || Type.IsEnum || Inline);

            foreach (object Value in List)
            {
                if (Pointers)
                {
                    AddReference(Type, new RefValue
                    {
                        Value = Value,
                        Position = BaseStream.Position
                    });

                    Skip(4);
                }
                else
                {
                    WriteValue(Value, true);
                }
            }

            if (HasBuffered) WriteBool();
        }

        private void WriteValue(RefValue Reference)
        {
            FieldInfo Info = Reference.Info;
            object Value = Reference.Value;
            bool Range = Info?.IsDefined(typeof(RangeAttribute)) ?? false;

            if (Value != null && (!(Value is IList) || ((IList)Value).Count > 0 || Range))
            {
                ObjectInfo OInfo = GetObjInfo(Value, Info);

                long Position = BaseStream.Position;

                if (OInfo.Position == Position)
                {
                    Reference.Serialize?.Invoke(this, Value);

                    AddObjPointer(Value, Position);
                    WriteValue(Value);
                }

                if (Reference.Position != -1)
                {
                    long EndPos = BaseStream.Position;

                    BaseStream.Seek(Reference.Position, SeekOrigin.Begin);

                    WritePointer(OInfo.Position);

                    if (Reference.HasLength)
                    {
                        if (Range)
                            WritePointer((uint)(OInfo.Length != 0 ? OInfo.Length : EndPos));
                        else
                            Writer.Write(((IList)Value).Count);
                    }

                    if (Reference.HasTwoPtr) WritePointer(OInfo.Position);

                    BaseStream.Seek(EndPos, SeekOrigin.Begin);
                }
            }
        }

        private void WritePointer(uint Pointer)
        {
            Pointers.Add(BaseStream.Position);
            Writer.Write(Pointer);
        }

        private ObjectInfo GetObjInfo(object Value, FieldInfo Info)
        {
            ObjectInfo Output = new ObjectInfo
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
                uint StartPos = 0;
                int EndPos = 0;
                int Matches = 0;

                foreach (object Elem in ((IList)Value))
                {
                    if (ObjPointers.ContainsKey(Elem) && (ObjPointers[Elem].Position == EndPos || EndPos == 0))
                    {
                        if (Matches++ == 0) EndPos = (int)(StartPos = ObjPointers[Elem].Position);
                    }
                    else
                    {
                        break;
                    }

                    EndPos += ObjPointers[Elem].Length;
                }

                if (Matches > 0 && Matches == ((IList)Value).Count)
                {
                    Output.Position = StartPos;
                    Output.Length = EndPos;
                }
            }

            return Output;
        }

        public void WriteObject(object Value)
        {
            Type ValueType = Value.GetType();

            int Index = Contents.Values.Count;

            if (Value is ICustomSerialization)
            {
                if (((ICustomSerialization)Value).Serialize(this)) return;
            }

            foreach (FieldInfo Info in ValueType.GetFields(Binding))
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
                        bool HasTwoPtr = Info.IsDefined(typeof(RepeatPointerAttribute));

                        RefValue Ref = new RefValue
                        {
                            Info = Info,
                            Value = Info.GetValue(Value),
                            Position = BaseStream.Position,
                            HasLength = HasLength,
                            HasTwoPtr = HasTwoPtr
                        };

                        if (Value is ICustomSerializeCmd && Type == typeof(uint[]))
                        {
                            Ref.Serialize = ((ICustomSerializeCmd)Value).SerializeCmd;
                        }

                        AddReference(Type, Ref);

                        Skip((HasLength ? 8 : 4) + (HasTwoPtr ? 4 : 0));
                    }
                }
            }

            if (ValueType.IsClass && !ValueType.IsDefined(typeof(InlineAttribute)))
            {
                while (Index < Contents.Values.Count)
                {
                    WriteValue(Contents.Values[Index]);
                    Contents.Values.RemoveAt(Index);
                }
            }
        }

        private void AddReference(Type Type, RefValue Ref)
        {
            if (Type == typeof(string))
                Strings.Values.Add(Ref);
            else if (Type == typeof(uint[]))
                Commands.Values.Add(Ref);
            else
                Contents.Values.Add(Ref);
        }

        public void Skip(int Bytes)
        {
            while (Bytes-- > 0) BaseStream.WriteByte(0);
        }
    }
}
