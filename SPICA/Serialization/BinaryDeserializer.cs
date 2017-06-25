using SPICA.Math3D;
using SPICA.Serialization.Attributes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SPICA.Serialization
{
    class BinaryDeserializer : BinarySerialization
    {
        public readonly BinaryReader Reader;

        private Dictionary<long, object> Objects;
        private Dictionary<long, object> ListObjs;

        public BinaryDeserializer(Stream BaseStream, SerializationOptions Options) : base(BaseStream, Options)
        {
            Reader = new BinaryReader(BaseStream);

            Objects  = new Dictionary<long, object>();
            ListObjs = new Dictionary<long, object>();
        }

        public T Deserialize<T>()
        {
            return (T)ReadValue(typeof(T));
        }

        private object ReadValue(Type Type, bool IsRef = false)
        {
            if (Type.IsPrimitive || Type.IsEnum)
            {
                switch (Type.GetTypeCode(Type))
                {
                    case TypeCode.UInt64:  return Reader.ReadUInt64();
                    case TypeCode.UInt32:  return Reader.ReadUInt32();
                    case TypeCode.UInt16:  return Reader.ReadUInt16();
                    case TypeCode.Byte:    return Reader.ReadByte();
                    case TypeCode.Int64:   return Reader.ReadInt64();
                    case TypeCode.Int32:   return Reader.ReadInt32();
                    case TypeCode.Int16:   return Reader.ReadInt16();
                    case TypeCode.SByte:   return Reader.ReadSByte();
                    case TypeCode.Single:  return Reader.ReadSingle();
                    case TypeCode.Double:  return Reader.ReadDouble();
                    case TypeCode.Boolean: return Reader.ReadUInt32() != 0;

                    default: return null;
                }
            }
            else if (IsList(Type))
            {
                return ReadList(Type);
            }
            else if (Type == typeof(string))
            {
                return ReadString();
            }
            else if (Type == typeof(Vector2))
            {
                return Reader.ReadVector2();
            }
            else if (Type == typeof(Vector3))
            {
                return Reader.ReadVector3();
            }
            else if (Type == typeof(Vector4))
            {
                return Reader.ReadVector4();
            }
            else if (Type == typeof(Quaternion))
            {
                return Reader.ReadQuaternion();
            }
            else if (Type == typeof(Matrix3x3))
            {
                return Reader.ReadMatrix3x3();
            }
            else if (Type == typeof(Matrix3x4))
            {
                return Reader.ReadMatrix3x4();
            }
            else if (Type == typeof(Matrix4x4))
            {
                return Reader.ReadMatrix4x4();
            }
            else
            {
                return ReadObject(Type, IsRef);
            }
        }

        private IList ReadList(Type Type)
        {
            return ReadList(Type, false, Reader.ReadInt32());
        }

        private IList ReadList(Type Type, FieldInfo Info)
        {
            return ReadList(
                Type,
                Info.IsDefined(typeof(RangeAttribute)),
                Info.GetCustomAttribute<FixedLengthAttribute>()?.Length ?? Reader.ReadInt32());
        }

        private IList ReadList(Type Type, bool Range, int Length)
        {
            IList List;

            if (Type.IsArray)
            {
                Type = Type.GetElementType();
                List = Array.CreateInstance(Type, Length);
            }
            else
            {
                List = (IList)Activator.CreateInstance(Type);
                Type = Type.GetGenericArguments()[0];
            }

            BitReader BR = new BitReader(Reader);

            bool IsBool  = Type == typeof(bool);
            bool Inline  = Type.IsDefined(typeof(InlineAttribute));
            bool IsValue = Type.IsValueType || Type.IsEnum || Inline;

            for (int Index = 0; (Range ? BaseStream.Position : Index) < Length; Index++)
            {
                long Position = BaseStream.Position;

                object Value;

                if (IsBool)
                {
                    Value = BR.ReadBit();
                }
                else if (IsValue)
                {
                    Value = ReadValue(Type);
                }
                else
                {
                    Value = ReadReference(Type);
                }

                /*
                 * This is not necessary to make deserialization work, but
                 * is needed because H3D uses range lists for the meshes,
                 * and since meshes are actually classes treated as structs,
                 * we need to use the same reference for meshes on the different layer
                 * lists, otherwise it writes the same mesh more than once (and
                 * this should still work, but the file will be bigger for no
                 * good reason, and also is not what the original tool does).
                 */
                if (Type.IsClass && !IsList(Type))
                {
                    if (!ListObjs.TryGetValue(Position, out object Obj))
                    {
                        ListObjs.Add(Position, Value);
                    }
                    else if (Range)
                    {
                        Value = Obj;
                    }
                }

                if (List.IsFixedSize)
                {
                    List[Index] = Value;
                }
                else
                {
                    List.Add(Value);
                }
            }

            return List;
        }

        private string ReadString()
        {
            StringBuilder SB = new StringBuilder();

            for (char Chr; (Chr = Reader.ReadChar()) != '\0';)
            {
                SB.Append(Chr);
            }

            return SB.ToString();
        }

        private object ReadObject(Type ObjectType, bool IsRef = false)
        {
            long Position = BaseStream.Position;

            if (ObjectType.IsDefined(typeof(TypeChoiceAttribute)))
            {
                uint TypeId = Reader.ReadUInt32();

                Type Type = GetMatchingType(ObjectType, TypeId);

                if (Type != null)
                {
                    ObjectType = Type;
                }
                else
                {
                    Debug.WriteLine(string.Format(
                        "[SPICA|BinaryDeserializer] Unknown Type Id 0x{0:x8} at address {1:x8} and class {2}!",
                        TypeId,
                        Position,
                        ObjectType.FullName));
                }
            }

            object Value = Activator.CreateInstance(ObjectType);

            if (IsRef) Objects.Add(Position, Value);

            int FieldsCount = 0;

            foreach (FieldInfo Info in GetFieldsSorted(ObjectType))
            {
                FieldsCount++;

                if (!Info.GetCustomAttribute<IfVersionAttribute>()?.Compare(FileVersion) ?? false) continue;

                if (!(
                    Info.IsDefined(typeof(IgnoreAttribute)) ||
                    Info.IsDefined(typeof(CompilerGeneratedAttribute))))
                {
                    Type Type = Info.FieldType;

                    string TCName = Info.GetCustomAttribute<TypeChoiceNameAttribute>()?.FieldName;

                    if (TCName != null && Info.IsDefined(typeof(TypeChoiceAttribute)))
                    {
                        FieldInfo TCInfo = ObjectType.GetField(TCName);

                        uint TypeId = Convert.ToUInt32(TCInfo.GetValue(Value));

                        Type = GetMatchingType(Info, TypeId) ?? Type;
                    }

                    bool Inline;

                    Inline  = Info.IsDefined(typeof(InlineAttribute));
                    Inline |= Type.IsDefined(typeof(InlineAttribute));

                    object FieldValue;

                    if (Type.IsValueType || Type.IsEnum || Inline)
                    {
                        FieldValue = IsList(Type)
                            ? ReadList(Type, Info)
                            : ReadValue(Type);

                        if (Type.IsPrimitive && Info.IsDefined(typeof(VersionAttribute)))
                        {
                            FileVersion = Convert.ToInt32(FieldValue);
                        }
                    }
                    else
                    {
                        FieldValue = ReadReference(Type, Info);
                    }

                    if (FieldValue != null) Info.SetValue(Value, FieldValue);

                    Align(Info.GetCustomAttribute<PaddingAttribute>()?.Size ?? 1);
                }
            }

            if (FieldsCount == 0)
            {
                Debug.WriteLine($"[SPICA|BinaryDeserializer] Class {ObjectType.FullName} has no accessible fields!");
            }

            if (Value is ICustomSerialization) ((ICustomSerialization)Value).Deserialize(this);

            return Value;
        }

        private Type GetMatchingType(MemberInfo Info, uint TypeId)
        {
            foreach (TypeChoiceAttribute Attr in Info.GetCustomAttributes<TypeChoiceAttribute>())
            {
                if (Attr.TypeVal == TypeId)
                {
                    return Attr.Type;
                }
            }

            return null;
        }

        private object ReadReference(Type Type, FieldInfo Info = null)
        {
            uint Address;
            int  Length;

            if (GetLengthPos(Info) == LengthPos.AfterPtr)
            {
                Address = ReadPointer();
                Length  = ReadLength(Type, Info);
            }
            else
            {
                Length  = ReadLength(Type, Info);
                Address = ReadPointer();
            }

            bool Range  = Info?.IsDefined(typeof(RangeAttribute))         ?? false;
            bool Repeat = Info?.IsDefined(typeof(RepeatPointerAttribute)) ?? false;

            if (Repeat) BaseStream.Seek(4, SeekOrigin.Current);

            object Value = null;

            if (Address != 0 && (!IsList(Type) || (IsList(Type) && Length > 0)))
            {
                if (!Objects.TryGetValue(Address, out Value))
                {
                    long Position = BaseStream.Position;

                    BaseStream.Seek(Address, SeekOrigin.Begin);

                    Value = IsList(Type)
                        ? ReadList(Type, Range, Length)
                        : ReadValue(Type, true);

                    BaseStream.Seek(Position, SeekOrigin.Begin);
                }
            }

            return Value;
        }

        private int ReadLength(Type Type, FieldInfo Info = null)
        {
            if (IsList(Type))
            {
                if (Info?.IsDefined(typeof(FixedLengthAttribute)) ?? false)
                {
                    return Info.GetCustomAttribute<FixedLengthAttribute>().Length;
                }
                else if (GetLengthSize(Info) == LengthSize.Short)
                {
                    return Reader.ReadUInt16();
                }
                else
                {
                    return Reader.ReadInt32();
                }
            }

            return 0;
        }

        public uint ReadPointer()
        {
            uint Address = Reader.ReadUInt32();

            if (Options.PtrType == PointerType.SelfRelative && Address != 0)
            {
                Address += (uint)BaseStream.Position - 4;
            }

            return Address;
        }
    }
}
