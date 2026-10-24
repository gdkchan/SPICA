using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public sealed class DAEExportOptions
    {
        public float FramesPerSecond { get; set; } = 30;
        public int SamplesPerFrame { get; set; } = 4;
        public float MatrixTolerance { get; set; } = 0.0001f;
        public int MaxSubdivisionDepth { get; set; } = 10;
        public bool ExportTextures { get; set; } = true;
        public bool ExportSkinning { get; set; }

        internal void Validate()
        {
            if (!float.IsFinite(FramesPerSecond) || FramesPerSecond <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(FramesPerSecond));
            }

            if (SamplesPerFrame < 1 || SamplesPerFrame > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(SamplesPerFrame));
            }

            if (!float.IsFinite(MatrixTolerance) || MatrixTolerance <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(MatrixTolerance));
            }

            if (MaxSubdivisionDepth < 0 || MaxSubdivisionDepth > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(MaxSubdivisionDepth));
            }
        }
    }

    public class DAEExtra
    {
        public DAEExtraTechnique technique = new DAEExtraTechnique();
    }

    public class DAEExtraTechnique
    {
        [XmlAttribute] public string profile = "SPICA";
        [XmlAnyElement] public List<XmlElement> data = new List<XmlElement>();
    }
}


