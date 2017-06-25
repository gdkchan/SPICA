using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace SPICA.Formats.CtrGfx
{
    public class GfxMetaDataSingle : GfxMetaData
    {
        [Inline] public readonly List<float> Values = new List<float>();
    }

    public class GfxMetaDataColor : GfxMetaData
    {
        [Inline] public readonly List<Vector4> Values = new List<Vector4>();
    }

    public class GfxMetaDataInteger : GfxMetaData
    {
        [Inline] public readonly List<int> Values = new List<int>();
    }

    #region "String types"
    //TODO: Support different string formats on the serializer, instead of encapsulating
    //a string in different types to define encoding. This is a bad solution.
    public class GfxStringUtf8 : ICustomSerialization
    {
        [Ignore] private string Str;

        public GfxStringUtf8() { }

        public GfxStringUtf8(string Str)
        {
            this.Str = Str;
        }

        public override string ToString()
        {
            return Str ?? string.Empty;
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Str = Deserializer.Reader.ReadNullTerminatedStringUtf8();
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Writer.WriteNullTerminatedStringUtf8(Str);

            return true;
        }
    }

    public class GfxStringUtf16LE : ICustomSerialization
    {
        [Ignore] private string Str;

        public GfxStringUtf16LE() { }

        public GfxStringUtf16LE(string Str)
        {
            this.Str = Str;
        }

        public override string ToString()
        {
            return Str ?? string.Empty;
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Str = Deserializer.Reader.ReadNullTerminatedStringUtf16LE();
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Writer.WriteNullTerminatedStringUtf16LE(Str);

            return true;
        }
    }

    public class GfxStringUtf16BE : ICustomSerialization
    {
        [Ignore] private string Str;

        public GfxStringUtf16BE() { }

        public GfxStringUtf16BE(string Str)
        {
            this.Str = Str;
        }

        public override string ToString()
        {
            return Str ?? string.Empty;
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Str = Deserializer.Reader.ReadNullTerminatedStringUtf16BE();
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Writer.WriteNullTerminatedStringUtf16BE(Str);

            return true;
        }
    }
    #endregion

    public class GfxMetaDataString : GfxMetaData
    {
        public GfxStringFormat Format;

        [Inline]
        [TypeChoiceName("Format")]
        [TypeChoice((uint)GfxStringFormat.Ascii,   typeof(List<string>))]
        [TypeChoice((uint)GfxStringFormat.Utf8,    typeof(List<GfxStringUtf8>))]
        [TypeChoice((uint)GfxStringFormat.Utf16LE, typeof(List<GfxStringUtf16LE>))]
        [TypeChoice((uint)GfxStringFormat.Utf16BE, typeof(List<GfxStringUtf16BE>))]
        public readonly IList Values;

        public GfxMetaDataString()
        {
            Values = new List<string>();
        }
    }

    [TypeChoice(0x10000000u, typeof(GfxMetaDataString))]
    [TypeChoice(0x20000000u, typeof(GfxMetaDataInteger))]
    [TypeChoice(0x40000000u, typeof(GfxMetaDataColor))]
    [TypeChoice(0x80000000u, typeof(GfxMetaDataSingle))]
    public class GfxMetaData : INamed
    {
        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value ?? throw Exceptions.GetNullException("Name");
            }
        }

        public GfxMetaDataType Type;
    }
}
