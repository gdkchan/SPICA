using System;
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

        protected class BitReader
        {
            private BinaryReader Reader;

            private uint Bools;
            private int  Index;

            public BitReader(BinaryReader Reader)
            {
                this.Reader = Reader;
            }

            public bool ReadBit()
            {
                if ((Index++ & 0x1f) == 0)
                {
                    Bools = Reader.ReadUInt32();
                }

                bool Value = (Bools & 1) != 0;

                Bools >>= 1;

                return Value;
            }
        }

        protected class BitWriter
        {
            private BinaryWriter Writer;

            private uint Bools;
            private int  Index;

            public BitWriter(BinaryWriter Writer)
            {
                this.Writer = Writer;
            }

            public void WriteBit(bool Value)
            {
                Bools |= ((Value ? 1u : 0u) << Index);

                if (++Index == 32)
                {
                    Writer.Write(Bools);

                    Index = 0;
                    Bools = 0;
                }
            }

            public void Flush()
            {
                if (Index != 0)
                {
                    Writer.Write(Bools);
                }
            }
        }

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
    }
}
