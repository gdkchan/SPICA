using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAEEffect
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        public DAEProfileCOMMON profile_COMMON = new DAEProfileCOMMON();
    }

    public class DAEProfileCOMMON
    {
        [XmlAttribute] public string sid;

        [XmlElement("newparam")] public List<DAEEffectParam> newparam = new List<DAEEffectParam>();

        public DAEEffectProfileCOMMONTechnique technique = new DAEEffectProfileCOMMONTechnique();
    }

    public class DAEEffectParam
    {
        [XmlAttribute] public string sid;

        [XmlElement(IsNullable = false)] public DAEEffectParamSurfaceElement surface;
        [XmlElement(IsNullable = false)] public DAEEffectParamSampler2DElement sampler2D;
    }

    public class DAEEffectParamSurfaceElement
    {
        [XmlAttribute] public string type;

        public string init_from;
        public string format;
    }

    public class DAEEffectParamSampler2DElement
    {
        public string source;
        public DAEWrap wrap_s;
        public DAEWrap wrap_t;
        public DAEFilter minfilter;
        public DAEFilter magfilter;
        public DAEFilter mipfilter;
    }

    public enum DAEWrap
    {
        NONE,
        WRAP,
        MIRROR,
        CLAMP,
        BORDER
    }

    public enum DAEFilter
    {
        NONE,
        NEAREST,
        LINEAR,
        NEAREST_MIPMAP_NEAREST,
        LINEAR_MIPMAP_NEAREST,
        NEAREST_MIPMAP_LINEAR,
        LINEAR_MIPMAP_LINEAR
    }

    public class DAEEffectProfileCOMMONTechnique
    {
        [XmlAttribute] public string sid;

        public DAEPhong phong = new DAEPhong();
    }

    public class DAEPhong
    {
        public DAEPhongDiffuse diffuse = new DAEPhongDiffuse();
    }

    public class DAEPhongDiffuse
    {
        public DAEPhongDiffuseTexture texture = new DAEPhongDiffuseTexture();
    }

    public class DAEPhongDiffuseTexture
    {
        [XmlAttribute] public string texture;
        [XmlAttribute] public string texcoord = "uv";
    }
}
