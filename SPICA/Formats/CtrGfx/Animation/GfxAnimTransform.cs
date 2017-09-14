using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System.IO;

namespace SPICA.Formats.CtrGfx.Animation
{
    public class GfxAnimTransform : ICustomSerialization
    {
        [Ignore] private GfxFloatKeyFrameGroup _ScaleX;
        [Ignore] private GfxFloatKeyFrameGroup _ScaleY;
        [Ignore] private GfxFloatKeyFrameGroup _ScaleZ;

        [Ignore] private GfxFloatKeyFrameGroup _RotationX;
        [Ignore] private GfxFloatKeyFrameGroup _RotationY;
        [Ignore] private GfxFloatKeyFrameGroup _RotationZ;

        [Ignore] private GfxFloatKeyFrameGroup _TranslationX;
        [Ignore] private GfxFloatKeyFrameGroup _TranslationY;
        [Ignore] private GfxFloatKeyFrameGroup _TranslationZ;

        public GfxFloatKeyFrameGroup ScaleX       => _ScaleX;
        public GfxFloatKeyFrameGroup ScaleY       => _ScaleY;
        public GfxFloatKeyFrameGroup ScaleZ       => _ScaleZ;

        public GfxFloatKeyFrameGroup RotationX    => _RotationX;
        public GfxFloatKeyFrameGroup RotationY    => _RotationY;
        public GfxFloatKeyFrameGroup RotationZ    => _RotationZ;

        public GfxFloatKeyFrameGroup TranslationX => _TranslationX;
        public GfxFloatKeyFrameGroup TranslationY => _TranslationY;
        public GfxFloatKeyFrameGroup TranslationZ => _TranslationZ;

        public bool ScaleExists       => _ScaleX.Exists       || _ScaleY.Exists       || _ScaleZ.Exists;

        public bool RotationExists    => _RotationX.Exists    || _RotationY.Exists    || _RotationZ.Exists;

        public bool TranslationExists => _TranslationX.Exists || _TranslationY.Exists || _TranslationZ.Exists;

        public GfxAnimTransform()
        {
            _ScaleX       = new GfxFloatKeyFrameGroup();
            _ScaleY       = new GfxFloatKeyFrameGroup();
            _ScaleZ       = new GfxFloatKeyFrameGroup();

            _RotationX    = new GfxFloatKeyFrameGroup();
            _RotationY    = new GfxFloatKeyFrameGroup();
            _RotationZ    = new GfxFloatKeyFrameGroup();

            _TranslationX = new GfxFloatKeyFrameGroup();
            _TranslationY = new GfxFloatKeyFrameGroup();
            _TranslationZ = new GfxFloatKeyFrameGroup();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            long Position = Deserializer.BaseStream.Position;

            uint Flags = GfxAnimVector.GetFlagsFromElem(Deserializer, Position);

            uint ConstantMask = (uint)GfxAnimTransformFlags.IsScaleXConstant;
            uint NotExistMask = (uint)GfxAnimTransformFlags.IsScaleXInexistent;

            for (int ElemIndex = 0; ElemIndex < 10; ElemIndex++)
            {
                Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);

                Position += 4;

                bool Constant = (Flags & ConstantMask) != 0;
                bool Exists   = (Flags & NotExistMask) == 0;

                if (Exists)
                {
                    GfxFloatKeyFrameGroup FrameGrp = GfxFloatKeyFrameGroup.ReadGroup(Deserializer, Constant);

                    switch (ElemIndex)
                    {
                        case 0: _ScaleX       = FrameGrp; break;
                        case 1: _ScaleY       = FrameGrp; break;
                        case 2: _ScaleZ       = FrameGrp; break;

                        case 3: _RotationX    = FrameGrp; break;
                        case 4: _RotationY    = FrameGrp; break;
                        case 5: _RotationZ    = FrameGrp; break;

                        case 7: _TranslationX = FrameGrp; break;
                        case 8: _TranslationY = FrameGrp; break;
                        case 9: _TranslationZ = FrameGrp; break;
                    }
                }

                ConstantMask <<= 1;
                NotExistMask <<= 1;
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            uint ConstantMask = (uint)GfxAnimTransformFlags.IsScaleXConstant;
            uint NotExistMask = (uint)GfxAnimTransformFlags.IsScaleXInexistent;

            long Position = Serializer.BaseStream.Position;

            GfxAnimTransformFlags Flags = 0;

            Serializer.Writer.Write(0u);

            for (int ElemIndex = 0; ElemIndex < 10; ElemIndex++)
            {
                GfxFloatKeyFrameGroup FrameGrp = null;

                switch (ElemIndex)
                {
                    case 0: FrameGrp = _ScaleX;       break;
                    case 1: FrameGrp = _ScaleY;       break;
                    case 2: FrameGrp = _ScaleZ;       break;

                    case 3: FrameGrp = _RotationX;    break;
                    case 4: FrameGrp = _RotationY;    break;
                    case 5: FrameGrp = _RotationZ;    break;

                    case 7: FrameGrp = _TranslationX; break;
                    case 8: FrameGrp = _TranslationY; break;
                    case 9: FrameGrp = _TranslationZ; break;
                }

                if (FrameGrp == null)
                {
                    Flags |= (GfxAnimTransformFlags)NotExistMask;

                    Serializer.Writer.Write(0u);
                }
                else if (FrameGrp.KeyFrames.Count == 1)
                {
                    Flags |= (GfxAnimTransformFlags)ConstantMask;

                    Serializer.Writer.Write(FrameGrp.KeyFrames[0].Value);
                }
                else
                {
                    if (FrameGrp.KeyFrames.Count > 1)
                    {
                        Serializer.Sections[(uint)GfxSectionId.Contents].Values.Add(new RefValue()
                        {
                            Value    = FrameGrp,
                            Position = Serializer.BaseStream.Position
                        });
                    }
                    else
                    {
                        Flags |= (GfxAnimTransformFlags)NotExistMask;
                    }

                    Serializer.Writer.Write(0u);
                }

                ConstantMask <<= 1;
                NotExistMask <<= 1;
            }

            GfxAnimVector.WriteFlagsToElem(Serializer, Position, (uint)Flags);

            Serializer.BaseStream.Seek(Position + 4 + 9 * 4, SeekOrigin.Begin);

            return true;
        }
    }
}
