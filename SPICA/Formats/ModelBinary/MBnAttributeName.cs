using SPICA.PICA.Commands;

using System;

namespace SPICA.Formats.ModelBinary
{
    public enum MBnAttributeName
    {
        Position,
        Normal,
        Color,
        TexCoord0,
        TexCoord1,
        BoneIndex,
        BoneWeight
    }

    public static class MBnAttributeNameExtensions
    {
        public static PICAAttributeName ToPICAAttributeName(this MBnAttributeName AttrName)
        {
            switch (AttrName)
            {
                case MBnAttributeName.Position:   return PICAAttributeName.Position;
                case MBnAttributeName.Normal:     return PICAAttributeName.Normal;
                case MBnAttributeName.Color:      return PICAAttributeName.Color;
                case MBnAttributeName.TexCoord0:  return PICAAttributeName.TexCoord0;
                case MBnAttributeName.TexCoord1:  return PICAAttributeName.TexCoord1;
                case MBnAttributeName.BoneIndex:  return PICAAttributeName.BoneIndex;
                case MBnAttributeName.BoneWeight: return PICAAttributeName.BoneWeight;

                default: throw new ArgumentException($"Invalid or unimplemented MBn Attribute name {AttrName}!");
            }
        }
    }
}
