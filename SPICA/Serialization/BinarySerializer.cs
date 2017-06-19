using SPICA.Formats.CtrH3D;
using SPICA.Math3D;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SPICA.Serialization
{
    class BinarySerializer : BinarySerialization
    {
        public readonly BinaryWriter Writer;

        public int PhysicalAddressCount { get; private set; }

        public struct ObjectInfo
        {
            public uint Position;
            public int  Length;

            public void SetEnd(long Position)
            {
                Length = (int)(Position - this.Position);
            }
        }

        public class Section
        {
            public List<RefValue> Values;
            public ObjectInfo     Info;

            public Section()
            {
                Values = new List<RefValue>();
            }
        }

        public readonly Section Contents;
        public readonly Section Strings;
        public readonly Section Commands;
        public readonly Section RawDataTex;
        public readonly Section RawDataVtx;
        public readonly Section RawExtTex;
        public readonly Section RawExtVtx;

        public readonly Dictionary<object, ObjectInfo> ObjPointers;

        public readonly List<long> Pointers;

        public readonly H3DRelocator Relocator;

        public BinarySerializer(Stream BaseStream, SerializationOptions Options, H3DRelocator Relocator = null) : base(BaseStream, Options)
        {
            this.Relocator = Relocator;

            Writer = new BinaryWriter(BaseStream);

            Contents   = new Section();
            Strings    = new Section();
            Commands   = new Section();
            RawDataTex = new Section();
            RawDataVtx = new Section();
            RawExtTex  = new Section();
            RawExtVtx  = new Section();

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
                    case TypeCode.UInt64:  Writer.Write((ulong)Value);          break;
                    case TypeCode.UInt32:  Writer.Write((uint)Value);           break;
                    case TypeCode.UInt16:  Writer.Write((ushort)Value);         break;
                    case TypeCode.Byte:    Writer.Write((byte)Value);           break;
                    case TypeCode.Int64:   Writer.Write((long)Value);           break;
                    case TypeCode.Int32:   Writer.Write((int)Value);            break;
                    case TypeCode.Int16:   Writer.Write((short)Value);          break;
                    case TypeCode.SByte:   Writer.Write((sbyte)Value);          break;
                    case TypeCode.Single:  Writer.Write((float)Value);          break;
                    case TypeCode.Double:  Writer.Write((double)Value);         break;
                    case TypeCode.Boolean: Writer.Write((bool)Value ? 1u : 0u); break;
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
            else if (Value is Vector2)
            {
                Writer.Write((Vector2)Value);
            }
            else if (Value is Vector3)
            {
                Writer.Write((Vector3)Value);
            }
            else if (Value is Vector4)
            {
                Writer.Write((Vector4)Value);
            }
            else if (Value is Quaternion)
            {
                Writer.Write((Quaternion)Value);
            }
            else if (Value is Matrix3x3)
            {
                Writer.Write((Matrix3x3)Value);
            }
            else if (Value is Matrix3x4)
            {
                Writer.Write((Matrix3x4)Value);
            }
            else if (Value is Matrix4x4)
            {
                Writer.Write((Matrix4x4)Value);
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
                    Length   = (int)(BaseStream.Position - Position)
                });
            }
        }

        private void WriteList(IList List)
        {
            if (List.Count == 0) return;

            //NOTE: This doesn't work as expected for Lists containing different types (not used through).
            //Assumes that the Type of the first element is the same for all other elements.
            Type Type = List[0].GetType();

            bool Inline = Type.IsDefined(typeof(InlineAttribute));
            bool Pointers = !(Type.IsValueType || Type.IsEnum || Inline);

            BitWriter BW = new BitWriter(Writer);

            foreach (object Value in List)
            {
                if (Pointers)
                {
                    AddReference(Type, new RefValue
                    {
                        Value    = Value,
                        Position = BaseStream.Position
                    });

                    Skip(4);
                }
                else if (Type == typeof(bool))
                {
                    BW.WriteBit((bool)Value);
                }
                else
                {
                    WriteValue(Value, true);
                }
            }

            BW.Flush();
        }

        private void WriteValue(RefValue Reference)
        {
            FieldInfo Info   = Reference.Info;
            object    Parent = Reference.Parent;
            object    Value  = Reference.Value;
            bool      Range  = Info?.IsDefined(typeof(RangeAttribute)) ?? false;

            if (Value != null && (!(Value is IList) || ((IList)Value).Count > 0 || Range))
            {
                ObjectInfo OInfo = GetObjInfo(Value, Info);

                long Position = BaseStream.Position;

                if (OInfo.Position == Position)
                {
                    if (Parent != null &&
                        Parent is ICustomSerializeCmd &&
                        Info.FieldType == typeof(uint[]))
                    {
                        ((ICustomSerializeCmd)Parent).SerializeCmd(this, Value);
                    }

                    AddObjPointer(Value, Position);
                    WriteValue(Value);
                }

                if (Reference.Position != -1)
                {
                    long EndPos = BaseStream.Position;

                    BaseStream.Seek(Reference.Position, SeekOrigin.Begin);

                    uint Pointer = OInfo.Position + Reference.PointerOffset;

                    if (Options.LenPos == LengthPos.AfterPointer)
                    {
                        WritePointer(Pointer);
                    }

                    if (Reference.HasLength)
                    {
                        if (Range)
                            WritePointer((uint)(OInfo.Length != 0 ? OInfo.Length : EndPos));
                        else
                            Writer.Write(((IList)Value).Count);
                    }

                    if (Options.LenPos == LengthPos.BeforePointer)
                    {
                        WritePointer(Pointer);
                    }

                    if (Reference.HasTwoPtr)
                    {
                        WritePointer(Pointer);
                    }

                    BaseStream.Seek(EndPos, SeekOrigin.Begin);
                }
            }
        }

        public void WritePointer(uint Pointer)
        {
            Pointers.Add(BaseStream.Position);

            if (Options.PtrType == PointerType.SelfRelative && Pointer != 0)
            {
                Pointer -= (uint)BaseStream.Position;
            }

            Writer.Write(Pointer);
        }

        private ObjectInfo GetObjInfo(object Value, FieldInfo Info)
        {
            ObjectInfo Output = new ObjectInfo
            {
                Position = (uint)BaseStream.Position,
                Length   = 0
            };

            if (ObjPointers.ContainsKey(Value))
            {
                Output = ObjPointers[Value];
            }
            else if (Value is IList)
            {
                //This is used to find lists with segments of already serialized values.
                //We can avoid storing them again if the same sequence is repeated.
                uint StartPos = 0;
                int  EndPos   = 0;
                int  Matches  = 0;

                foreach (object Elem in ((IList)Value))
                {
                    if (ObjPointers.ContainsKey(Elem) && (
                        EndPos == ObjPointers[Elem].Position ||
                        EndPos == 0))
                    {
                        if (Matches++ == 0)
                        {
                            EndPos = (int)(StartPos = ObjPointers[Elem].Position);
                        }
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
                    Output.Length   = EndPos;
                }
            }

            return Output;
        }

        private void WriteObject(object Value)
        {
            Type ValueType = Value.GetType();

            if (ValueType.IsDefined(typeof(TypeChoiceAttribute)))
            {
                foreach (TypeChoiceAttribute Attr in ValueType.GetCustomAttributes<TypeChoiceAttribute>())
                {
                    if (Attr.Type == ValueType)
                    {
                        Writer.Write(Attr.TypeId);

                        break;
                    }
                }
            }

            int Index = Contents.Values.Count;

            if (Value is ICustomSerialization)
            {
                if (((ICustomSerialization)Value).Serialize(this)) return;
            }

            foreach (FieldInfo Info in GetFieldsSorted(ValueType))
            {
                if (!Info.GetCustomAttribute<IfVersionAttribute>()?.Compare(FileVersion) ?? false) continue;

                if (!(
                    Info.IsDefined(typeof(IgnoreAttribute)) ||
                    Info.IsDefined(typeof(CompilerGeneratedAttribute))))
                {
                    Type Type = Info.FieldType;

                    bool Inline;

                    Inline  = Info.IsDefined(typeof(InlineAttribute));
                    Inline |= Type.IsDefined(typeof(InlineAttribute));

                    if (Type.IsValueType || Type.IsEnum || Inline)
                    {
                        object FieldValue = Info.GetValue(Value);

                        if (Info.IsDefined(typeof(VersionAttribute)) && Type.IsPrimitive)
                        {
                            FileVersion = Convert.ToInt32(FieldValue);
                        }

                        WriteValue(FieldValue);
                    }
                    else
                    {
                        bool IsList = typeof(IList).IsAssignableFrom(Type);
                        bool HasLength = !Info.IsDefined(typeof(FixedLengthAttribute)) && IsList;
                        bool HasTwoPtr = Info.IsDefined(typeof(RepeatPointerAttribute));

                        RefValue Ref = new RefValue
                        {
                            Parent    = Value,
                            Info      = Info,
                            Value     = Info.GetValue(Value),
                            Position  = BaseStream.Position,
                            HasLength = HasLength,
                            HasTwoPtr = HasTwoPtr
                        };

                        AddReference(Type, Ref);

                        Skip((HasLength ? 8 : 4) + (HasTwoPtr ? 4 : 0));
                    }

                    if (Info.IsDefined(typeof(PaddingAttribute)))
                    {
                        int Size = Info.GetCustomAttribute<PaddingAttribute>().Size;

                        while ((BaseStream.Position % Size) != 0) BaseStream.WriteByte(0);
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
            if (Ref.Info?.IsDefined(typeof(SectionAttribute)) ?? false)
            {
                SectionAttribute Attr = Ref.Info.GetCustomAttribute<SectionAttribute>();

                switch (Attr.Name)
                {
                    case SectionName.Contents:   Contents.Values.Add(Ref);   break;
                    case SectionName.Strings:    Strings.Values.Add(Ref);    break;
                    case SectionName.Commands:   Commands.Values.Add(Ref);   break;
                    case SectionName.RawDataTex: RawDataTex.Values.Add(Ref); break;
                    case SectionName.RawDataVtx: RawDataVtx.Values.Add(Ref); break;
                    case SectionName.RawExtTex:  RawExtTex.Values.Add(Ref);  break;
                    case SectionName.RawExtVtx:  RawExtVtx.Values.Add(Ref);  break;
                }
            }
            else if (Type == typeof(string))
            {
                Strings.Values.Add(Ref);
            }
            else if (Type == typeof(uint[]))
            {
                Commands.Values.Add(Ref);
            }
            else
            {
                Contents.Values.Add(Ref);
            }
        }

        public void Skip(int Bytes)
        {
            while (Bytes-- > 0) BaseStream.WriteByte(0);
        }
    }
}
