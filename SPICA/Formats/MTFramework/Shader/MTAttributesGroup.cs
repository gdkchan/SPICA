using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.MTFramework.Shader
{
    class MTAttributesGroup
    {
        private static float[] Scales = 
        {
            1f / sbyte.MaxValue,
            1f / byte.MaxValue,
            1f / short.MaxValue,
            1
        };

        public PICAAttribute[] Attributes;

        public MTAttributesGroup() { }

        public MTAttributesGroup(BinaryReader Reader, uint StringsTblAddr)
        {
            byte   AttrId    = Reader.ReadByte(); //?
            ushort Length    = Reader.ReadUInt16();
            byte   WordCount = Reader.ReadByte();
            uint   Padding   = Reader.ReadUInt32();

            Attributes = new PICAAttribute[WordCount >> 1];

            for (int j = 0; j < Attributes.Length; j++)
            {
                string Name = MTShaderEffects.GetName(Reader, StringsTblAddr);

                uint Format = Reader.ReadUInt32();

                bool IsNormalized = (Format & 0x40) != 0; //When true number is in 0/1 range (or -1/1 when below is false?)
                bool IsUnsigned   = (Format & 0x80) != 0; //When true number is unsigned (no negative values)? Not sure

                uint AttrIndex  = (Format >>  0) & 0x3f;
                uint AttrFormat = (Format >>  8) & 0x7;
                uint AttrElems  = (Format >> 11) & 0x3;
                uint AttrOffset = (Format >> 24) & 0xff;

                AttrOffset *= 4;

                PICAAttribute Attr = new PICAAttribute();

                switch (Name.ToLower())
                {
                    case "position": Attr.Name = PICAAttributeName.Position;   break;
                    case "normal":   Attr.Name = PICAAttributeName.Normal;     break;
                    case "tangent":  Attr.Name = PICAAttributeName.Tangent;    break;
                    case "color":    Attr.Name = PICAAttributeName.Color;      break;
                    case "joint":    Attr.Name = PICAAttributeName.BoneIndex;  break;
                    case "weight":   Attr.Name = PICAAttributeName.BoneWeight; break;
                    case "texcoord":
                        if (AttrIndex < 3)
                            Attr.Name = (PICAAttributeName)((uint)PICAAttributeName.TexCoord0 + AttrIndex);
                        else
                            Attr.Name = PICAAttributeName.UserAttribute0; break;
                    default: Attr.Name = PICAAttributeName.UserAttribute0; break;
                }

                switch (AttrFormat)
                {
                    case 0: Attr.Format = PICAAttributeFormat.Float; break;
                    case 1: Attr.Format = PICAAttributeFormat.Short; break;
                    case 2: Attr.Format = PICAAttributeFormat.Byte;  break;
                    case 3: Attr.Format = PICAAttributeFormat.Ubyte; break;
                }

                if (IsNormalized)
                    Attr.Scale = Scales[(int)Attr.Format];
                else
                    Attr.Scale = 1;

                Attr.Elements = (int)AttrElems;

                Attributes[j] = Attr;
            }
        }
    }
}
