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

        private struct NamePointer
        {
            public string Name;
            public object Data;
            public long Position;
            public Type Type;
        }

        private List<NamePointer> Pointers;
        private List<NamePointer> Counts;
        private List<NamePointer> SectPtr;
        private List<NamePointer> SectLen;

        private struct SectionField
        {
            public string Name;
            public object Data;
            public int Order;
            public FieldInfo FInfo;
        }

        private List<SectionField> SectFlds;

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

            Pointers = new List<NamePointer>();
            Counts = new List<NamePointer>();
            SectPtr = new List<NamePointer>();
            SectLen = new List<NamePointer>();

            SectFlds = new List<SectionField>();

            StrTable = new Dictionary<string, long>();
        }

        /// <summary>
        ///     Serializes Data to the output Stream.
        /// </summary>
        /// <param name="Data">The Data to be serialized</param>
        public void Serialize(object Data)
        {
            Type DataType = Data.GetType();
            long Position = BaseStream.Position;

            //Write all Fields for this Object
            foreach (FieldInfo FInfo in DataType.GetFields(Binding)) WriteField(Data, FInfo);

            //Write pointers that points to this Object
            FindWrite(Pointers, DataType.Name, Position, IsSelfRel);

            //Make sure Booleans has been written
            if (HasBuffered)
            {
                HasBuffered = false;
                Writer.Write(BufferedUInt);
            }

            //This check and writes all Sections for the Class, Struct or Field
            if (DataType.IsDefined(typeof(SectionAttribute)))
            {
                foreach (SectionAttribute Attr in DataType.GetCustomAttributes<SectionAttribute>())
                {
                    WriteSection(Attr.Name, Attr.Align);
                }
            }
        }

        private void WriteSection(string Name, uint Align)
        {
            long Position = BaseStream.Position;

            for (int Order = 0; ; Order++)
            {
                Predicate<SectionField> SFldCurr = x => x.Name == Name && x.Order == Order;
                Predicate<SectionField> SFldNext = x => x.Name == Name && x.Order > Order;

                List<SectionField> SFlds = SectFlds.FindAll(SFldCurr);

                foreach (SectionField SFld in SFlds)
                {
                    WriteField(SFld.Data, SFld.FInfo, Name);
                    SectFlds.Remove(SFld);
                }

                if (!SectFlds.Exists(SFldNext)) break;
            }

            //Writes total length of the Section in bytes and Pointers to this Section
            long Length = BaseStream.Position - Position;

            if (Relocator != null) Relocator.AddSection(Position, Length, Name);

            while ((BaseStream.Position % Align) != 0) BaseStream.WriteByte(0);

            FindWrite(SectLen, Name, Length);
            FindWrite(SectPtr, Name, Position, IsSelfRel);
        }

        private void FindWrite(List<NamePointer> Ptrs, string Name, long Value, bool IsSelfRel = false)
        {
            Predicate<NamePointer> Pred = x => x.Name == Name;

            while (Ptrs.Exists(Pred))
            {
                NamePointer Ptr = Ptrs.Find(Pred);

                WriteValue(Value - (IsSelfRel ? Ptr.Position : 0), Ptr.Position, Ptr.Type);
                Ptrs.Remove(Ptr);
            }
        }

        private void WriteField(object Data, FieldInfo FInfo, string Section = null)
        {
            Type DataType = Data.GetType();
            Type FType = FInfo.FieldType;
            object Value = FInfo.GetValue(Data);

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
                    SectFlds.Add(new SectionField
                    {
                        Name = Attr.Name,
                        Order = Attr.Order,
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

            bool SkipVal = Value == null;

            //Make sure that all Booleans are written if next type is not a bool
            if (HasBuffered && FType != typeof(bool))
            {
                HasBuffered = false;
                Writer.Write(BufferedUInt);
            }

            //Writes all pointers that points to this Object
            PointerOfAttribute PtrAttr = FInfo.GetCustomAttribute<PointerOfAttribute>();

            Predicate<NamePointer> RefPred = x => x.Data == Data && x.Name == FInfo.Name && !x.Type.IsArray;
            Predicate<NamePointer> TblPred = x => x.Data == Data && x.Name == FInfo.Name && x.Type.IsArray;

            while (Pointers.Exists(RefPred))
            {
                NamePointer Ptr = Pointers.Find(RefPred);

                long Address = 0;

                if (FInfo.IsDefined(typeof(PointerOfAttribute)) && FType.IsArray)
                {
                    SkipVal = DataType.GetField(PtrAttr.ObjName, Binding).GetValue(Data) == null;
                }

                if (!SkipVal)
                {
                    if (SkipVal = (Value is string && StrTable.ContainsKey((string)Value)))
                    {
                        Address = StrTable[(string)Value] - (IsSelfRel ? Ptr.Position : 0);
                    }
                    else
                    {
                        Address = BaseStream.Position - (IsSelfRel ? Ptr.Position : 0);
                    }
                }

                WriteValue(Address, Ptr.Position, Ptr.Type);
                Pointers.Remove(Ptr);
            }

            //Pointer and Count/Length Attributes
            if (FInfo.IsDefined(typeof(PointerOfAttribute)))
            {
                AddNamePointer(Pointers, PtrAttr.ObjName, Data, FType);

                if (FType.IsArray)
                {
                    if (!SkipVal)
                    {
                        Type ArrType = FType.GetElementType();
                        Array Array = (Array)DataType.GetField(PtrAttr.ObjName, Binding).GetValue(Data);
                        int Length = GetBytesLength(Array.Length, ArrType);

                        if (Relocator != null)
                        {
                            for (int Offset = 0; Offset < Length; Offset += GetBytesLength(1, ArrType))
                            {
                                Relocator.AddPointer(BaseStream.Position + Offset);
                            }
                        }
                        
                        while (Length-- > 0) BaseStream.WriteByte(0);
                    }
                    
                    return;
                }
                else if (Relocator != null)
                {
                    Relocator.AddPointer(BaseStream.Position);
                }
            }
            else if (FInfo.IsDefined(typeof(CountOfAttribute)))
            {
                AddNamePointer(Counts, FInfo.GetCustomAttributes<CountOfAttribute>().First().ArrName, Data, FType);
            }
            else if (FInfo.IsDefined(typeof(SectionPointerOfAttribute)))
            {
                AddNamePointer(SectPtr, FInfo.GetCustomAttribute<SectionPointerOfAttribute>().Name, null, FType);
            }
            else if (FInfo.IsDefined(typeof(SectionLengthOfAttribute)))
            {
                AddNamePointer(SectLen, FInfo.GetCustomAttribute<SectionLengthOfAttribute>().Name, null, FType);
            }

            //Write value to output
            if (FType.IsArray && !SkipVal)
            {
                Array Array = (Array)Value;

                while (Counts.Exists(RefPred))
                {
                    NamePointer Cnt = Counts.Find(RefPred);

                    WriteValue(CalcCount(Data, FInfo.Name, Array.Length), Cnt.Position, Cnt.Type);
                    Counts.Remove(Cnt);
                }

                List<NamePointer> PtrTable = Pointers.FindAll(TblPred);
                if (PtrTable.Count > 0) Pointers.RemoveAll(TblPred);

                if (PtrTable.Count == 0 && Array is byte[])
                {
                    Writer.Write((byte[])Array);
                }
                else
                {
                    for (int Index = 0; Index < Array.Length; Index++)
                    {
                        object Elem = Array.GetValue(Index);

                        if (Elem != null)
                        {
                            foreach (NamePointer Ptr in PtrTable)
                            {
                                Type PtrType = Ptr.Type.GetElementType();
                                int PtrOffset = GetBytesLength(Index, PtrType);
                                long PtrAddress = Ptr.Position + PtrOffset;

                                long Address = 0;

                                if (SkipVal = (Elem is string && StrTable.ContainsKey((string)Elem)))
                                {
                                    Address = StrTable[(string)Elem] - (IsSelfRel ? PtrAddress : 0);
                                }
                                else
                                {
                                    Address = BaseStream.Position - (IsSelfRel ? PtrAddress : 0);
                                }

                                WriteValue(Address, PtrAddress, PtrType);
                            }

                            if (!SkipVal) WriteValue(Elem);
                        }
                    }

                    //If the Fixed Count is greater than the actual Count of the Array, fill the rest with zeros
                    if (FInfo.IsDefined(typeof(FixedCountAttribute)))
                    {
                        int Diff = FInfo.GetCustomAttribute<FixedCountAttribute>().Count - Array.Length;
                        int DiffBytes = GetBytesLength(Diff, Array.GetType().GetElementType());
                        while (DiffBytes-- > 0) BaseStream.WriteByte(0);
                    }
                }
            }
            else if (!SkipVal)
            {
                WriteValue(Value);
            }
        }

        private int CalcCount(object Data, string Name, int Count)
        {
            //Searches all Fields of the current Object trying to find a matching CountOf Attribute
            //It then calculates the correct Count decremented (maybe there's a better way to do this?)
            FieldInfo[] Fields = Data.GetType().GetFields();

            foreach (FieldInfo FInfo in Fields.Where(x => x.IsDefined(typeof(CountOfAttribute))))
            {
                foreach (CountOfAttribute Attr in FInfo.GetCustomAttributes<CountOfAttribute>())
                {
                    if (Attr.ArrName == Name) return Count - Attr.Increment;
                }
            }

            return Count;
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

        private void AddNamePointer(List<NamePointer> Target, string Name, object Data, Type FType)
        {
            Target.Add(new NamePointer { Name = Name, Data = Data, Position = BaseStream.Position, Type = FType });
        }

        public void AddPointer(string Name, object Data, long Position, Type FType)
        {
            Pointers.Add(new NamePointer { Name = Name, Data = Data, Position = Position, Type = FType });
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

        private void WriteValue(long Value, long Position, Type FType)
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

            BaseStream.Seek(OldPosition, SeekOrigin.Begin);
        }
    }
}
