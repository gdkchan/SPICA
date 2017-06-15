using SPICA.Formats.Common;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System.IO;
using System.Numerics;

namespace SPICA.Formats.CtrH3D
{
    public struct H3DVertexDataAttribute : ICustomSerialization
    {
        private byte _Name;

        public PICAAttributeName Name
        {
            get
            {
                return (PICAAttributeName)_Name;
            }
            set
            {
                _Name = (byte)value;
            }
        }

        private byte Type;

        public PICAAttributeFormat Format
        {
            get
            {
                return (PICAAttributeFormat)BitUtils.GetBits(Type, 0, 2);
            }
            set
            {
                Type = BitUtils.SetBits(Type, (uint)value, 0, 2);
            }
        }

        public int Elements
        {
            get
            {
                return (int)BitUtils.GetBits(Type, 2, 2) + 1;
            }
            set
            {
                if (value < 1)
                {
                    throw Exceptions.GetLessThanException("Elements", 1);
                }

                if (value > 4)
                {
                    throw Exceptions.GetGreaterThanException("Elements", 4);
                }

                Type = BitUtils.SetBits(Type, (uint)value - 1, 2, 2);
            }
        }

        [Padding(2)] public byte Stride;

        public bool IsFixed { get { return Stride == 0; } }

        private uint _Offset;

        public int Offset
        {
            get
            {
                return IsFixed ? 0 : (int)_Offset;
            }
            set
            {
                _Offset = (uint)value;
            }
        }

        [Ignore] public Vector4 FixedValue;

        [Ignore] internal byte[] RawBuffer;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            if (IsFixed)
            {
                long Position = Deserializer.BaseStream.Position;

                Deserializer.BaseStream.Seek(_Offset, SeekOrigin.Begin);

                ReadFixedValue(Deserializer.Reader);

                Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            if (IsFixed)
            {
                Serializer.Contents.Values.Add(new RefValue
                {
                    Position      = Serializer.BaseStream.Position + 4,
                    Value         = GetFixedValueBuffer(),
                    PointerOffset = 0
                });
            }
            else
            {
                Serializer.RawDataVtx.Values.Add(new RefValue
                {
                    Position      = Serializer.BaseStream.Position + 4,
                    Value         = RawBuffer,
                    PointerOffset = _Offset
                });
            }

            return false;
        }

        private void ReadFixedValue(BinaryReader Reader)
        {
            float[] v = new float[4];

            for (int i = 0; i < Elements; i++)
            {
                switch (Format)
                {
                    case PICAAttributeFormat.Byte:  v[i] = Reader.ReadSByte();  break;
                    case PICAAttributeFormat.Ubyte: v[i] = Reader.ReadByte();   break;
                    case PICAAttributeFormat.Short: v[i] = Reader.ReadInt16();  break;
                    case PICAAttributeFormat.Float: v[i] = Reader.ReadSingle(); break;
                }
            }

            FixedValue = new Vector4(v[0], v[1], v[2], v[3]);
        }

        private byte[] GetFixedValueBuffer()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                float[] v = new float[4];

                FixedValue.CopyTo(v);

                for (int i = 0; i < Elements; i++)
                {
                    switch (Format)
                    {
                        case PICAAttributeFormat.Byte:  Writer.Write((sbyte)v[i]); break;
                        case PICAAttributeFormat.Ubyte: Writer.Write((byte)v[i]);  break;
                        case PICAAttributeFormat.Short: Writer.Write((short)v[i]); break;
                        case PICAAttributeFormat.Float: Writer.Write(v[i]);        break;
                    }
                }

                Writer.Align(4, 0);

                return MS.ToArray();
            }
        }
    }
}
