using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.MTFramework.Shader
{
    public class MTAttributesGroup : MTShaderEffect
    {
        private enum AttributeFormat
        {
            F32 = 1,
            S16 = 3,
            U16 = 4,
            S16N = 5,
            U16N = 6,
            S8 = 7,
            U8 = 8,
            S8N = 9,
            U8N = 10,
            RGB = 13,
            RGBA = 14
        }

        private static float[] Scales = 
        {
            1f / sbyte.MaxValue,
            1f / byte.MaxValue,
            1f / short.MaxValue,
            1
        };

        public readonly List<PICAAttribute> Attributes;

        public MTAttributesGroup() { }

        public MTAttributesGroup(BinaryReader Reader, uint StringsTblAddr)
        {
            byte   AttrId    = Reader.ReadByte(); //?
            ushort AttrCount = Reader.ReadUInt16(); //Bits 4-15 = Total attributes count
            byte   Stride    = Reader.ReadByte(); //In Word Count (32-bits)
            uint   Padding   = Reader.ReadUInt32(); //Not sure but seems to be always 0 so padding I guess

            AttrCount >>= 4;

            Stride *= 4;

            int RealOffset = 0;

            Attributes = new List<PICAAttribute>();

            for (int j = 0; j < AttrCount; j++)
            {
                string Name = MTShaderEffects.GetName(Reader, StringsTblAddr);

                uint Format = Reader.ReadUInt32();

                uint AttrIndex  = (Format >>  0) & 0x3f; //Used when attribute is repeated usually
                uint AttrFormat = (Format >>  6) & 0x1f; //See AttributeFormat enumerator
                uint AttrElems  = (Format >> 11) & 0x3; //2 = 2D, 3 = 3D, 4 = 4D, ...
                uint AttrOffset = (Format >> 24) & 0xff; //In Word Count (32-bits)

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

                bool Norm = false;

                int Size = 1;

                switch ((AttributeFormat)AttrFormat)
                {
                    case AttributeFormat.F32:  Attr.Format = PICAAttributeFormat.Float; Norm = false; Size = 4; break;
                    case AttributeFormat.S16:  Attr.Format = PICAAttributeFormat.Short; Norm = false; Size = 2; break;
                    case AttributeFormat.S16N: Attr.Format = PICAAttributeFormat.Short; Norm = true;  Size = 2; break;
                    case AttributeFormat.S8:   Attr.Format = PICAAttributeFormat.Byte;  Norm = false; Size = 1; break;
                    case AttributeFormat.U8:   Attr.Format = PICAAttributeFormat.Ubyte; Norm = false; Size = 1; break;
                    case AttributeFormat.S8N:  Attr.Format = PICAAttributeFormat.Byte;  Norm = true;  Size = 1; break;
                    case AttributeFormat.U8N:  Attr.Format = PICAAttributeFormat.Ubyte; Norm = true;  Size = 1; break;
                    case AttributeFormat.RGB:  Attr.Format = PICAAttributeFormat.Ubyte; Norm = true;  Size = 1; break;
                    case AttributeFormat.RGBA: Attr.Format = PICAAttributeFormat.Ubyte; Norm = true;  Size = 1; break;
                }

                Attr.Scale = Norm ? Scales[(int)Attr.Format] : 1;

                Attr.Elements = (int)AttrElems;

                Attributes.Add(Attr);

                RealOffset += (int)(Size * AttrElems);

                if (RealOffset >= Stride) break;
            }
        }
    }
}
