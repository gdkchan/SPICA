using SPICA.Serialization.BinaryAttributes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SPICA.Serialization
{
    /// <summary>
    ///     A Binary Deserializer that can be used to deserialize Data from Binary files into Objects.
    /// </summary>
    class BinaryDeserializer
    {
        public Stream BaseStream;
        public BinaryReader Reader;

        private bool IsSelfRel;

        //This is used to read Booleans
        private long BufferedPos = 0;
        private uint BufferedUInt = 0;
        private uint BufferedShift = 0;

        /// <summary>
        ///     Creates a new instance of the Binary Deserializer.
        /// </summary>
        /// <param name="BaseStream">The Base Stream from where the Data will be deserialized</param>
        public BinaryDeserializer(Stream BaseStream, bool IsSelfRel = false)
        {
            this.IsSelfRel = IsSelfRel;
            this.BaseStream = BaseStream;

            Reader = new BinaryReader(BaseStream);
        }

        /// <summary>
        ///     Deserializes Data from the input Stream.
        /// </summary>
        /// <typeparam name="T">The Type of the Data being deserialized</typeparam>
        /// <returns>An Object with the deserialized Data</returns>
        public T Deserialize<T>()
        {
            return (T)Deserialize(typeof(T));
        }

        /// <summary>
        ///     Deserializes Data from the input Stream.
        /// </summary>
        /// <param name="Data">The Type of the Data being deserialized</param>
        /// <returns>An Object with the deserialized Data</returns>
        public object Deserialize(Type OType)
        {
            const BindingFlags Binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            object Data = Activator.CreateInstance(OType);

            FieldInfo[] Fields = OType.GetFields(Binding);

            Dictionary<string, long> Pointers = new Dictionary<string, long>();
            Dictionary<string, long> Counts = new Dictionary<string, long>();
            Dictionary<string, Array> PtrTable = new Dictionary<string, Array>();

            foreach (FieldInfo FInfo in Fields)
            {
                if (FInfo.IsDefined(typeof(NonSerializedAttribute)))
                {
                    continue;
                }

                //Try to find a Pointer to this Object and seek to the correct position if found
                long Position = 0;
                bool HasPointer;

                if (HasPointer = Pointers.ContainsKey(FInfo.Name))
                {
                    long Address = Pointers[FInfo.Name];

                    Pointers.Remove(FInfo.Name);

                    //If it's a Null pointer then the Object doesn't exist
                    if (Address == 0) continue;

                    Position = BaseStream.Position;
                    BaseStream.Seek(Address, SeekOrigin.Begin);
                }

                if (FInfo.IsDefined(typeof(PointerOfAttribute)))
                {
                    PeekAdd(Pointers, FInfo.GetCustomAttribute<PointerOfAttribute>().ObjName, FInfo.FieldType);
                }
                else if (FInfo.IsDefined(typeof(CountOfAttribute)))
                {
                    foreach (CountOfAttribute Attr in FInfo.GetCustomAttributes<CountOfAttribute>())
                    {
                        PeekAdd(Counts, Attr.ArrName, FInfo.FieldType, Attr.Increment);
                    }
                }

                //Read field value from input
                if (FInfo.FieldType.IsArray)
                {
                    bool HasCount = Counts.ContainsKey(FInfo.Name);
                    bool HasFixedCnt = FInfo.IsDefined(typeof(FixedCountAttribute));
                    bool IsPtrTable = FInfo.IsDefined(typeof(PointerOfAttribute));
                    bool HasLength = IsPtrTable && !(HasCount || HasFixedCnt);

                    long PtrTableLength = 0;

                    if (HasLength)
                    {
                        PointerOfAttribute Attr = FInfo.GetCustomAttribute<PointerOfAttribute>();
                        FieldInfo TargetArrFld = Data.GetType().GetField(Attr.ObjName, Binding);

                        if (HasFixedCnt = TargetArrFld.IsDefined(typeof(FixedCountAttribute)))
                        {
                            PtrTableLength = TargetArrFld.GetCustomAttribute<FixedCountAttribute>().Count;
                        }
                        else if (HasCount = Counts.ContainsKey(Attr.ObjName))
                        {
                            PtrTableLength = Counts[Attr.ObjName];
                        }
                    }

                    if (HasCount || HasFixedCnt)
                    {
                        long Length = PtrTableLength;

                        if (!HasLength)
                        {
                            if (HasFixedCnt)
                            {
                                FixedCountAttribute Attr = FInfo.GetCustomAttribute<FixedCountAttribute>();
                                Length = Attr.Count;
                            }
                            else
                            {
                                Length = Counts[FInfo.Name];
                                Counts.Remove(FInfo.Name);
                            }
                        }

                        if (Length > 0)
                        {
                            Type ArrType = FInfo.FieldType.GetElementType();
                            Array Array = Array.CreateInstance(ArrType, Length);

                            if (PtrTable.ContainsKey(FInfo.Name))
                            {
                                Array Ptr = PtrTable[FInfo.Name];
                                Type PtrType = Ptr.GetType().GetElementType();

                                long OldPosition = BaseStream.Position;

                                for (int Index = 0; Index < Length; Index++)
                                {
                                    object Address = Ptr.GetValue(Index);

                                    switch (Type.GetTypeCode(PtrType))
                                    {
                                        case TypeCode.UInt32: BaseStream.Seek((uint)Address, SeekOrigin.Begin); break;
                                        case TypeCode.UInt16: BaseStream.Seek((ushort)Address, SeekOrigin.Begin); break;
                                        case TypeCode.Byte: BaseStream.Seek((byte)Address, SeekOrigin.Begin); break;
                                        case TypeCode.Int32: BaseStream.Seek((int)Address, SeekOrigin.Begin); break;
                                        case TypeCode.Int16: BaseStream.Seek((short)Address, SeekOrigin.Begin); break;
                                        case TypeCode.SByte: BaseStream.Seek((sbyte)Address, SeekOrigin.Begin); break;
                                    }

                                    if (BaseStream.Position != 0) Array.SetValue(ReadValue(Reader, ArrType), Index);
                                }

                                BaseStream.Seek(OldPosition, SeekOrigin.Begin);

                                PtrTable.Remove(FInfo.Name);
                            }
                            else
                            {
                                if (ArrType == typeof(byte))
                                {
                                    Array = Reader.ReadBytes((int)Length);
                                }
                                else
                                {
                                    for (int Index = 0; Index < Length; Index++)
                                    {
                                        Array.SetValue(ReadValue(Reader, ArrType), Index);
                                    }
                                }
                            }

                            FInfo.SetValue(Data, Array);

                            if (IsPtrTable)
                            {
                                PtrTable.Add(FInfo.GetCustomAttribute<PointerOfAttribute>().ObjName, Array);
                            }
                        }
                    }
                }
                else
                {
                    FInfo.SetValue(Data, ReadValue(Reader, FInfo.FieldType));
                }

                if (Data is ICustomDeserializer && FInfo.IsDefined(typeof(CustomSerializationAttribute)))
                {
                    ((ICustomDeserializer)Data).Deserialize(this, FInfo.Name);
                }

                //If we used a Pointer, then Seek back to where we was
                if (HasPointer) BaseStream.Seek(Position, SeekOrigin.Begin);
            }

            BufferedShift = 0;

            return Data;
        }

        private object ReadValue(BinaryReader Reader, Type FType)
        {
            if ((FType.IsValueType && FType.IsPrimitive) || FType.IsEnum)
            {
                switch (Type.GetTypeCode(FType))
                {
                    case TypeCode.UInt64: return Reader.ReadUInt64();
                    case TypeCode.UInt32: return Reader.ReadUInt32();
                    case TypeCode.UInt16: return Reader.ReadUInt16();
                    case TypeCode.Byte: return Reader.ReadByte();
                    case TypeCode.Int64: return Reader.ReadInt64();
                    case TypeCode.Int32: return Reader.ReadInt32();
                    case TypeCode.Int16: return Reader.ReadInt16();
                    case TypeCode.SByte: return Reader.ReadSByte();
                    case TypeCode.Single: return Reader.ReadSingle();
                    case TypeCode.Double: return Reader.ReadDouble();

                    case TypeCode.Boolean:
                        if (BufferedPos != BaseStream.Position || BufferedShift == 0)
                        {
                            BufferedUInt = Reader.ReadUInt32();
                            BufferedPos = BaseStream.Position;
                            BufferedShift = 32;
                        }

                        bool Value = (BufferedUInt & 1) != 0;
                        BufferedUInt >>= 1;
                        BufferedShift--;

                        return Value;

                    default: return null;
                }
            }
            else if (FType == typeof(string))
            {
                StringBuilder SB = new StringBuilder();

                char Chr;
                while ((Chr = Reader.ReadChar()) != '\0')
                {
                    SB.Append(Chr);
                }

                return SB.ToString();
            }
            else
            {
                return Deserialize(FType);
            }
        }

        private void PeekAdd(Dictionary<string, long> Target, string Key, Type FType, int Increment = 0)
        {
            if (Target.ContainsKey(Key)) return;

            long Position = BaseStream.Position;

            switch (Type.GetTypeCode(FType))
            {
                case TypeCode.UInt32: Target.Add(Key, Reader.ReadUInt32() + Increment); break;
                case TypeCode.Int32: Target.Add(Key, Reader.ReadInt32() + Increment); break;
                case TypeCode.UInt16: Target.Add(Key, Reader.ReadUInt16() + Increment); break;
                case TypeCode.Int16: Target.Add(Key, Reader.ReadInt16() + Increment); break;
                case TypeCode.Byte: Target.Add(Key, Reader.ReadByte() + Increment); break;
                case TypeCode.SByte: Target.Add(Key, Reader.ReadSByte() + Increment); break;
            }

            BaseStream.Seek(Position, SeekOrigin.Begin);
        }
    }
}
