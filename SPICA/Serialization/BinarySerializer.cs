using SPICA.Serialization.BinaryAttributes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SPICA.Serialization
{
    /// <summary>
    ///     A Binary Serializer that can be used to serialize Objects into Binary files.
    /// </summary>
    class BinarySerializer
    {
        private const BindingFlags Binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public Stream BaseStream;
        public BinaryWriter Writer;
        public IRelocator Relocator;

        private bool IsSelfRel;

        //Those are Pointers to a Pointer or Count, using a Reference or Name to identify the Element
        private struct NamePointer
        {
            public string Name;
            public long Position;
            public Type Type;
        }

        private struct RefPointer
        {
            public object ObjRef;
            public long Position;
            public Type Type;
        }

        private List<NamePointer> SecLen;
        private List<NamePointer> SecPtr;
        private List<RefPointer> Pointers;
        private List<RefPointer> PtrTable;

        private struct SectionField
        {
            public string Name;
            public int Prio;
            public object Data;
            public FieldInfo FInfo;
        }

        private List<SectionField> SecFlds;

        private Dictionary<string, long> StrTable;

        private bool HasBuffered = false;
        private uint BufferedUInt = 0;

        /// <summary>
        ///     Creates a new instance of the Binary Serializer.
        /// </summary>
        /// <param name="BaseStream">The Base Stream where the Data will be serialized</param>
        public BinarySerializer(Stream BaseStream, IRelocator Relocator = null, bool IsSelfRel = false)
        {
            this.BaseStream = BaseStream;
            this.Relocator = Relocator;
            this.IsSelfRel = IsSelfRel;

            Writer = new BinaryWriter(BaseStream);
            
            SecLen = new List<NamePointer>();
            SecPtr = new List<NamePointer>();
            Pointers = new List<RefPointer>();
            PtrTable = new List<RefPointer>();

            SecFlds = new List<SectionField>();

            StrTable = new Dictionary<string, long>();
        }

        /// <summary>
        ///     Serializes Data to the output Stream.
        /// </summary>
        /// <param name="Data">The Data to be serialized</param>
        public void Serialize(object Data)
        {
            Type DataType = Data.GetType();

            //Write all Fields for this Object
            foreach (FieldInfo FInfo in DataType.GetFields(Binding))
            {
                WriteField(Data, FInfo);
            }

            //Make sure Booleans has been written
            if (HasBuffered)
            {
                HasBuffered = false;
                Writer.Write(BufferedUInt);
            }

            CheckSection(DataType);
        }

        private void CheckSection(MemberInfo Info)
        {
            //This check and writes all Sections for the Class, Struct or Field
            if (Info.IsDefined(typeof(SectionAttribute)))
            {
                foreach (SectionAttribute Attr in Info.GetCustomAttributes<SectionAttribute>())
                {
                    WriteSection(Attr.Name, Attr.Align);
                }
            }
        }

        private void WriteSection(string Name, uint Align)
        {
            long Position = BaseStream.Position;

            for (int Prio = 0; ; Prio++)
            {
                Predicate<SectionField> SFldCurr = SFld => SFld.Name == Name && SFld.Prio == Prio;
                Predicate<SectionField> SFldNext = SFld => SFld.Name == Name && SFld.Prio > Prio;

                List<SectionField> SFlds = SecFlds.FindAll(SFldCurr);

                foreach (SectionField SFld in SFlds)
                {
                    WriteField(SFld.Data, SFld.FInfo, Name);
                    SecFlds.Remove(SFld);
                }

                if (!SecFlds.Exists(SFldNext)) break;
            }

            //Writes total length of the Section in bytes and Pointers to this Section
            long Length = BaseStream.Position - Position;

            if (Relocator != null) Relocator.AddSection(Position, Length, Name);

            while ((BaseStream.Position % Align) != 0) BaseStream.WriteByte(0);

            FindWrite(SecLen, Name, Length);
            FindWrite(SecPtr, Name, Position);
        }

        private void FindWrite(List<NamePointer> Ptrs, string Name, long Value)
        {
            Predicate<NamePointer> Pred = NP => NP.Name == Name;

            if (Ptrs.Exists(Pred))
            {
                NamePointer Ptr = Ptrs.Find(Pred);

                WriteValue(Value, Ptr.Position, Ptr.Type);
                Ptrs.Remove(Ptr);
            }
        }

        private void WriteField(object Data, FieldInfo FInfo, string Section = null)
        {
            Type FType = FInfo.FieldType;
            object Value = FInfo.GetValue(Data);
            bool IsPtrTable = FInfo.IsDefined(typeof(PointerOfAttribute)) && FType.IsArray;

            if (FInfo.IsDefined(typeof(NonSerializedAttribute)))
            {
                return;
            }
            else if (FInfo.IsDefined(typeof(TargetSectionAttribute)))
            {
                //This belongs to a section, so we delay the write and store the Field and Value on the Section list
                TargetSectionAttribute Attr = FInfo.GetCustomAttribute<TargetSectionAttribute>();

                if (Section != Attr.Name)
                {
                    SecFlds.Add(new SectionField
                    {
                        Name = Attr.Name,
                        Prio = Attr.Prio,
                        Data = Data,
                        FInfo = FInfo
                    });

                    return;
                }
            }

            if (Data is ICustomSerializer && FInfo.IsDefined(typeof(CustomSerializationAttribute)))
            {
                Value = ((ICustomSerializer)Data).Serialize(this, FInfo.Name);
            }

            //Make sure that all Booleans are written if next type is not a bool
            if (HasBuffered && FType != typeof(bool))
            {
                HasBuffered = false;
                Writer.Write(BufferedUInt);
            }

            //Writes all pointers that points to this Object
            bool WriteVal = Value != null;

            Predicate<RefPointer> PtrPred = Ptr => Ptr.ObjRef == Value;

            while (Pointers.Exists(PtrPred))
            {
                RefPointer Ptr = Pointers.Find(PtrPred);

                long Address = 0;
                long BaseOffs = IsSelfRel ? Ptr.Position : 0;

                if (WriteVal || IsPtrTable)
                {
                    Address = BaseStream.Position - BaseOffs;

                    if (Value is string && StrTable.ContainsKey((string)Value))
                    {
                        Address = StrTable[(string)Value] - BaseOffs;
                        WriteVal = false;
                    }
                }

                WriteValue(Address, Ptr.Position, Ptr.Type);
                Pointers.Remove(Ptr);
            }

            //Pointer and Count/Length Attributes
            if (FInfo.IsDefined(typeof(CountOfAttribute)))
            {
                CountOfAttribute Attr = FInfo.GetCustomAttributes<CountOfAttribute>().First();

                FieldInfo TargetFld = Data.GetType().GetField(Attr.ArrName, Binding);

                if (TargetFld != null && TargetFld.FieldType.IsArray)
                {
                    Array TargetArr = (Array)TargetFld.GetValue(Data);

                    if (TargetArr != null)
                    {
                        long Length = TargetArr.Length - Attr.Increment;
                        WriteValue(Length, BaseStream.Position, FType);
                    }
                }
            }
            else if (FInfo.IsDefined(typeof(SectionLengthOfAttribute)))
            {
                AddNamePointer(SecLen, FInfo.GetCustomAttribute<SectionLengthOfAttribute>().Name, FType);
            }
            else if (FInfo.IsDefined(typeof(SectionPointerOfAttribute)))
            {
                AddNamePointer(SecPtr, FInfo.GetCustomAttribute<SectionPointerOfAttribute>().Name, FType);
            }
            else if (FInfo.IsDefined(typeof(PointerOfAttribute)))
            {
                //This is an Object Pointer, so we store it on the to be written Pointer list
                PointerOfAttribute Attr = FInfo.GetCustomAttribute<PointerOfAttribute>();

                FieldInfo TargetFld = Data.GetType().GetField(Attr.ObjName, Binding);

                if (TargetFld != null)
                {
                    RefPointer Ptr = new RefPointer()
                    {
                        ObjRef = TargetFld.GetValue(Data),
                        Position = BaseStream.Position,
                        Type = FType
                    };

                    if (Relocator != null) Relocator.AddPointer(Ptr.Position);

                    if (IsPtrTable)
                    {
                        object TargetArr;

                        if ((TargetArr = TargetFld.GetValue(Data)) != null)
                        {
                            int Length = GetBytesLength(((Array)TargetArr).Length, FType.GetElementType());
                            while (Length-- > 0) BaseStream.WriteByte(0);
                        }

                        PtrTable.Add(Ptr);
                    }
                    else
                    {
                        Pointers.Add(Ptr);
                    }
                }
            }

            //Write value to output
            if (FType.IsArray)
            {
                if (!IsPtrTable && WriteVal)
                {
                    Array Array = (Array)Value;

                    RefPointer ArrPtr = PtrTable.Find(PtrPred);
                    bool HasPtrTable = PtrTable.Exists(PtrPred);

                    if (!HasPtrTable && Array is byte[])
                    {
                        Writer.Write((byte[])Array);
                    }
                    else
                    {
                        foreach (object Elem in Array)
                        {
                            WriteVal = Elem != null;

                            if (HasPtrTable)
                            {
                                long Address = 0;
                                long BaseOffs = IsSelfRel ? ArrPtr.Position : 0;

                                if (WriteVal)
                                {
                                    Address = BaseStream.Position - BaseOffs;

                                    if (Elem is string && StrTable.ContainsKey((string)Elem))
                                    {
                                        Address = StrTable[(string)Elem] - BaseOffs;
                                        WriteVal = false;
                                    }
                                }

                                ArrPtr.Position = WriteValue(Address, ArrPtr.Position, ArrPtr.Type.GetElementType());
                            }

                            if (WriteVal) WriteValue(Elem);
                        }

                        //If the Fixed Count is greater than the actual Count of the Array, fill the rest with zeros
                        if (FInfo.IsDefined(typeof(FixedCountAttribute)))
                        {
                            int Diff = FInfo.GetCustomAttribute<FixedCountAttribute>().Count - Array.Length;
                            int DiffBytes = GetBytesLength(Diff, Array.GetType().GetElementType());
                            while (DiffBytes-- > 0) BaseStream.WriteByte(0);
                        }
                    }

                    if (HasPtrTable) PtrTable.Remove(ArrPtr);
                }
            }
            else if (WriteVal)
            {
                WriteValue(Value);
            }

            CheckSection(FInfo);
        }

        //Helper functions
        private int GetBytesLength(int Length, Type ElemType)
        {
            switch (Type.GetTypeCode(ElemType))
            {
                case TypeCode.Double:
                case TypeCode.UInt64:
                case TypeCode.Int64:
                    Length <<= 3;
                    break;
                case TypeCode.Single:
                case TypeCode.UInt32:
                case TypeCode.Int32:
                    Length <<= 2;
                    break;
                case TypeCode.UInt16:
                case TypeCode.Int16:
                    Length <<= 1;
                    break;
                case TypeCode.Byte:
                case TypeCode.SByte:
                    Length <<= 0;
                    break;

                default: Length = 0; break;
            }

            return Length;
        }

        private void AddNamePointer(List<NamePointer> Target, string Name, Type Type)
        {
            Target.Add(new NamePointer { Name = Name, Position = BaseStream.Position, Type = Type });
        }

        public void AddPointer(object ObjRef, long Position, Type Type)
        {
            Pointers.Add(new RefPointer { ObjRef = ObjRef, Position = Position, Type = Type });
        }

        //Value write functions
        private void WriteValue(object Value)
        {
            Type FType = Value.GetType();

            if ((FType.IsValueType && FType.IsPrimitive) || FType.IsEnum)
            {
                switch (Type.GetTypeCode(FType))
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
                        break;
                }
            }
            else if (Value is string)
            {
                string Str = (string)Value;

                if (!StrTable.ContainsKey(Str)) StrTable.Add(Str, BaseStream.Position);

                foreach (char Chr in Str)
                {
                    Writer.Write(Chr);
                }

                Writer.Write('\0');
            }
            else
            {
                Serialize(Value);
            }
        }

        private long WriteValue(long Value, long Position, Type FType)
        {
            long OldPosition = BaseStream.Position;

            BaseStream.Seek(Position, SeekOrigin.Begin);

            switch (Type.GetTypeCode(FType))
            {
                case TypeCode.UInt32:
                case TypeCode.Int32:
                    Writer.Write((uint)Value);
                    break;
                case TypeCode.UInt16:
                case TypeCode.Int16:
                    Writer.Write((ushort)Value);
                    break;
                case TypeCode.Byte:
                case TypeCode.SByte:
                    Writer.Write((byte)Value);
                    break;
            }

            Position = BaseStream.Position;

            BaseStream.Seek(OldPosition, SeekOrigin.Begin);

            return Position;
        }
    }
}
