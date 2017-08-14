using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;
using System.Collections;
using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D
{
    [Inline]
    public class H3DMetaDataValue : INamed
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public H3DMetaDataType Type;

        [TypeChoiceName("Type")]
        [TypeChoice((uint)H3DMetaDataType.Integer,       typeof(List<int>))]
        [TypeChoice((uint)H3DMetaDataType.Single,        typeof(List<float>))]
        [TypeChoice((uint)H3DMetaDataType.ASCIIString,   typeof(List<string>))]
        [TypeChoice((uint)H3DMetaDataType.UnicodeString, typeof(List<H3DStringUtf16>))]
        [TypeChoice((uint)H3DMetaDataType.BoundingBox,   typeof(List<H3DBoundingBox>))]
        [TypeChoice((uint)H3DMetaDataType.VertexData,    typeof(List<H3DVertexData>))]
        [CustomLength(LengthPos.BeforePtr, LengthSize.Short)]
        public IList Values;

        public H3DMetaDataValue() { }

        public H3DMetaDataValue(string Name, params int[] Values)
        {
            H3DMetaDataValueImpl(Name, H3DMetaDataType.Integer, Values);
        }

        public H3DMetaDataValue(string Name, params float[] Values)
        {
            H3DMetaDataValueImpl(Name, H3DMetaDataType.Single, Values);
        }

        public H3DMetaDataValue(string Name, params string[] Values)
        {
            H3DMetaDataValueImpl(Name, H3DMetaDataType.ASCIIString, Values);
        }

        public H3DMetaDataValue(string Name, params H3DStringUtf16[] Values)
        {
            H3DMetaDataValueImpl(Name, H3DMetaDataType.UnicodeString, Values);
        }

        public H3DMetaDataValue(string Name, params H3DBoundingBox[] Values)
        {
            H3DMetaDataValueImpl(Name, H3DMetaDataType.BoundingBox, Values);
        }

        public H3DMetaDataValue(string Name, params H3DVertexData[] Values)
        {
            H3DMetaDataValueImpl(Name, H3DMetaDataType.VertexData, Values);
        }

        private void H3DMetaDataValueImpl<T>(string Name, H3DMetaDataType Type, params T[] Values)
        {
            if (Values.Length == 0)
            {
                throw new ArgumentException($"You must specify at least one value!");
            }

            _Name = $"${Name}";

            this.Type = Type;

            this.Values = new List<T>(Values);
        }

        public H3DMetaDataValue(H3DBoundingBox OBB)
        {
            _Name = "OBBox";

            Type = H3DMetaDataType.BoundingBox;

            Values = new List<H3DBoundingBox> { OBB };
        }
    }
}
