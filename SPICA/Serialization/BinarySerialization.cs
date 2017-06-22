using SPICA.Serialization.Attributes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SPICA.Serialization
{
    class BinarySerialization
    {
        public readonly Stream BaseStream;

        protected SerializationOptions Options;

        public int FileVersion;

        private const BindingFlags Binding =
            BindingFlags.DeclaredOnly |
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        public BinarySerialization(Stream BaseStream, SerializationOptions Options)
        {
            this.BaseStream = BaseStream;
            this.Options    = Options;
        }

        protected IEnumerable<FieldInfo> GetFieldsSorted(Type ObjectType)
        {
            Stack<Type> TypeStack = new Stack<Type>();

            do
            {
                TypeStack.Push(ObjectType);

                ObjectType = ObjectType.BaseType;
            }
            while (ObjectType != null);

            while (TypeStack.Count > 0)
            {
                ObjectType = TypeStack.Pop();

                foreach (FieldInfo Info in ObjectType.GetFields(Binding))
                {
                    yield return Info;
                }
            }
        }

        protected void Align(int BlockSize)
        {
            long Remainder = BaseStream.Position % BlockSize;

            if (Remainder != 0)
            {
                BaseStream.Seek(BlockSize - Remainder, SeekOrigin.Current);
            }
        }

        protected bool IsList(Type Type)
        {
            return typeof(IList).IsAssignableFrom(Type);
        }

        protected LengthPos GetLengthPos(FieldInfo Info = null)
        {
            return Info?.GetCustomAttribute<CustomLengthAttribute>()?.Pos ?? Options.LenPos;
        }

        protected LengthSize GetLengthSize(FieldInfo Info = null)
        {
            return Info?.GetCustomAttribute<CustomLengthAttribute>()?.Size ?? LengthSize.Integer;
        }

        protected int GetIntLengthSize(FieldInfo Info = null)
        {
            return GetLengthSize(Info) == LengthSize.Short ? 2 : 4;
        }
    }
}
