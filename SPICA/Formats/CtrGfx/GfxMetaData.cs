using SPICA.Formats.Common;
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
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public GfxMetaDataType Type;
    }
}
