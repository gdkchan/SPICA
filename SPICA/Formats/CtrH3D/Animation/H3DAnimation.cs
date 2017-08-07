using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimation : INamed
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public H3DAnimationFlags AnimationFlags;
        public H3DAnimationType  AnimationType;

        public ushort CurvesCount;

        public float FramesCount;

        public readonly List<H3DAnimationElement> Elements;

        public H3DMetaData MetaData;

        [Ignore] private Dictionary<string, H3DAnimationElement> CachedElements;

        public H3DAnimation()
        {
            Elements = new List<H3DAnimationElement>();

            CachedElements = new Dictionary<string, H3DAnimationElement>();
        }

        public H3DAnimationElement GetElement(string Name)
        {
            if (!CachedElements.TryGetValue(Name, out H3DAnimationElement Output))
            {
                foreach (H3DAnimationElement Elem in Elements)
                {
                    if (Elem.Name == Name)
                    {
                        Output = Elem;

                        break;
                    }
                }

                if (Output != null)
                {
                    CachedElements.Add(Name, Output);
                }
            }

            if (Output != null && Output.Name != Name)
            {
                CachedElements.Clear();

                Output = GetElement(Name);
            }

            return Output;
        }
    }
}
