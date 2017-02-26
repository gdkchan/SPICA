using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.Formats.CtrH3D.LUT
{
    public class H3DLUT : INamed
    {
        public List<H3DLUTSampler> Samplers;

        private string _Name;

        [XmlAttribute]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public static H3DLUT CelShading
        {
            get
            {
                //TODO: Lots of hardcoded values, maybe make then user configurable?
                /*
                 * This build a LUT with a exponential value that goes 0->1
                 * The exponential value is obtained by Value ^ 2 (Value * Value), where Value is initially linear
                 * It is then quantized by ceil(Value * 4) / 4, which gives the step shade effect
                 * The ceil function rounds the value to a integer, and after the division most precision is lost
                 */
                H3DLUTSampler Sampler = new H3DLUTSampler();

                Sampler.Name = "SpecSampler";

                for (int Index = 0; Index < 128; Index++)
                {
                    double Value = Index / 127d;

                    Value = Math.Ceiling(Value * Value * 4) * 0.25f;

                    Sampler.Table[Index ^ 0x7f] = (float)Value;
                    Sampler.Table[Index | 0x80] = 1;
                }

                H3DLUT Output = new H3DLUT();

                Output.Name = "SpecTable";

                Output.Samplers.Add(Sampler);

                return Output;
            }
        }

        public H3DLUT()
        {
            Samplers = new List<H3DLUTSampler>();
        }
    }
}
