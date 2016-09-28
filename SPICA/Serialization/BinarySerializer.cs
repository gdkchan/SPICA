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

        private bool IsSelfRel;

        public Stream BaseStream;
        public BinaryWriter Writer;

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
            public object Value;
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
        public BinarySerializer(Stream BaseStream, bool IsSelfRel = false)
        {
            this.IsSelfRel = IsSelfRel;
            this.BaseStream = BaseStream;

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

            //Call Custom Serialization if existent
            if (Data is ICustomSerializer)
            {
                ((ICustomSerializer)Data).Serialize(this);
            }

            //Write all Fields for this Object
            foreach (FieldInfo FInfo in DataType.GetFields(Binding))
            {
                WriteField(Data, FInfo.GetValue(Data), FInfo);
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
                    WriteSection(Attr.Name);
                }
            }
        }

        private void WriteSection(string Name)
        {
            long Position = BaseStream.Position;

            for (int Prio = 0; ; Prio++)
            {
                List<SectionField> SFlds = SecFlds.FindAll(SFld => SFld.Name == Name && SFld.Prio == Prio);

                if (SecFlds.Where(SFld => SFld.Name == Name && SFld.Prio >= Prio).Count() == 0) break;

                foreach (SectionField SFld in SFlds)
                {
                    WriteField(SFld.Data, SFld.Value, SFld.FInfo, Name);
                    SecFlds.Remove(SFld);
                }
            }

            //Writes total length of the Section in bytes and Pointers to this Section
            long Length = BaseStream.Position - Position;

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

        private void WriteField(object Data, object Value, FieldInfo FInfo, string Section = null)
        {
            Type FType = FInfo.FieldType;
            bool IsPtrTable = false;

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
                        Value = Value,
                        FInfo = FInfo
                    });

                    return;
                }
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

            if (Pointers.Exists(PtrPred))
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

                    if (IsPtrTable = FType.IsArray)
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

            //Write Padding if needed
            if (FInfo.IsDefined(typeof(AlignAttribute)))
            {
                while ((BaseStream.Position & 0xf) != 0) BaseStream.WriteByte(0);
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
