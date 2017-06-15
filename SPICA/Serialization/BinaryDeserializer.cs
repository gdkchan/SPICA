using SPICA.Math3D;
using SPICA.Serialization.Attributes;

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
    class BinaryDeserializer
    {
        public readonly Stream BaseStream;
        public readonly BinaryReader Reader;

        private struct ObjectInfo
        {
            public long Position;
            public Type ObjectType;
        }

        private class ObjectRef
        {
            public object Value;
            public int Length;
        }

        private Dictionary<ObjectInfo, ObjectRef> ObjPointers;

        private SerializationOptions Options;

        public int FileVersion;

        private const BindingFlags Binding =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        public BinaryDeserializer(Stream BaseStream, SerializationOptions Options)
        {
            this.BaseStream = BaseStream;
            this.Options    = Options;

            Reader = new BinaryReader(BaseStream);

            ObjPointers = new Dictionary<ObjectInfo, ObjectRef>();
        }

        public T Deserialize<T>()
        {
            return (T)ReadValue(typeof(T));
        }

        private object ReadValue(Type Type, FieldInfo Info = null, int Length = 0)
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
                    case TypeCode.Boolean: return Reader.ReadByte() != 0;

                    default: return null;
                }
            }
            else if (typeof(IList).IsAssignableFrom(Type))
            {
                return ReadList(Type, Info, Length);
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
                return ReadObject(Type);
            }
        }

        private IList ReadList(Type Type, FieldInfo Info, int Length)
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

            long Position = BaseStream.Position;
            bool Range = Info?.IsDefined(typeof(RangeAttribute)) ?? false;
            bool Inline = Type.IsDefined(typeof(InlineAttribute));
            bool Pointers = !(Type.IsValueType || Type.IsEnum || Inline);

            uint Bools = 0;

            int Index;

            for (Index = 0; (Range ? BaseStream.Position : Index) < Length; Index++)
            {
                if (Pointers)
                {
                    BaseStream.Seek(Position + Index * 4, SeekOrigin.Begin);

                    uint Address = ReadPointer();

                    if (Address == 0)
                    {
                        if (!List.IsFixedSize) List.Add(null);

                        continue;
                    }

                    BaseStream.Seek(Address, SeekOrigin.Begin);
                }

                object Value;

                if (Type == typeof(bool))
                {
                    if ((Index & 0x1f) == 0)
                    {
                        Bools = Reader.ReadUInt32();
                    }

                    Value = (Bools & 1) != 0;

                    Bools >>= 1;
                }
                else
                {
                    Value = ReadValue(Type);
                }

                if (List.IsFixedSize)
                    List[Index] = Value;
                else
                    List.Add(Value);
            }

            if (Pointers)
            {
                BaseStream.Seek(Position + Index * 4, SeekOrigin.Begin);
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

        private object ReadObject(Type ObjectType)
        {
            object Value = Activator.CreateInstance(ObjectType);
            object OldVal = Value;

            ObjectInfo OInfo = new ObjectInfo
            {
                Position   = BaseStream.Position,
                ObjectType = ObjectType
            };

            if (ObjPointers.ContainsKey(OInfo))
            {
                Value = ObjPointers[OInfo].Value;

                BaseStream.Seek(ObjPointers[OInfo].Length, SeekOrigin.Current);
            }
            else
            {
                ObjectRef ORef = new ObjectRef { Value = Value };

                ObjPointers.Add(OInfo, ORef);

                long Position = BaseStream.Position;

                foreach (FieldInfo Info in ObjectType.GetFields(Binding))
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
                            object FieldValue = ReadValue(Type, Info, Info.IsDefined(typeof(FixedLengthAttribute))
                                ? Info.GetCustomAttribute<FixedLengthAttribute>().Length
                                : 0);

                            if (Info.IsDefined(typeof(VersionAttribute)) && Type.IsPrimitive)
                            {
                                FileVersion = Convert.ToInt32(FieldValue);
                            }

                            Info.SetValue(Value, FieldValue);
                        }
                        else
                        {
                            ReadReference(Value, Info);
                        }

                        if (Info.IsDefined(typeof(PaddingAttribute)))
                        {
                            int Size = Info.GetCustomAttribute<PaddingAttribute>().Size;

                            while ((BaseStream.Position % Size) != 0) BaseStream.WriteByte(0);
                        }
                    }
                }

                if (Value is ICustomSerialization) ((ICustomSerialization)Value).Deserialize(this);

                ORef.Length = (int)(BaseStream.Position - Position);
            }

            return Value;
        }

        private void ReadReference(object Parent, FieldInfo Info)
        {
            uint Address;
            int  Length;

            if (Options.LenPos == LengthPos.AfterPointer)
            {
                Address = ReadPointer();
                Length  = ReadLength(Info);
            }
            else
            {
                Length  = ReadLength(Info);
                Address = ReadPointer();
            }

            if (Info.IsDefined(typeof(RepeatPointerAttribute))) Reader.ReadUInt32();

            if (Address != 0)
            {
                long Position = BaseStream.Position;

                BaseStream.Seek(Address, SeekOrigin.Begin);

                Info.SetValue(Parent, ReadValue(Info.FieldType, Info, Length));

                BaseStream.Seek(Position, SeekOrigin.Begin);
            }
        }

        private int ReadLength(FieldInfo Info)
        {
            if (Info.IsDefined(typeof(FixedLengthAttribute)))
            {
                return Info.GetCustomAttribute<FixedLengthAttribute>().Length;
            }
            else if (typeof(IList).IsAssignableFrom(Info.FieldType))
            {
                return Reader.ReadInt32();
            }
            else
            {
                return 0;
            }
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
