using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System.IO;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimTransform : ICustomSerialization
    {
        private H3DAnimTransformFlags Flags;

        [Ignore] private H3DFloatKeyFrameGroup _ScaleX;
        [Ignore] private H3DFloatKeyFrameGroup _ScaleY;
        [Ignore] private H3DFloatKeyFrameGroup _ScaleZ;

        [Ignore] private H3DFloatKeyFrameGroup _RotationX;
        [Ignore] private H3DFloatKeyFrameGroup _RotationY;
        [Ignore] private H3DFloatKeyFrameGroup _RotationZ;

        [Ignore] private H3DFloatKeyFrameGroup _TranslationX;
        [Ignore] private H3DFloatKeyFrameGroup _TranslationY;
        [Ignore] private H3DFloatKeyFrameGroup _TranslationZ;

        public H3DFloatKeyFrameGroup ScaleX       => _ScaleX;
        public H3DFloatKeyFrameGroup ScaleY       => _ScaleY;
        public H3DFloatKeyFrameGroup ScaleZ       => _ScaleZ;

        public H3DFloatKeyFrameGroup RotationX    => _RotationX;
        public H3DFloatKeyFrameGroup RotationY    => _RotationY;
        public H3DFloatKeyFrameGroup RotationZ    => _RotationZ;

        public H3DFloatKeyFrameGroup TranslationX => _TranslationX;
        public H3DFloatKeyFrameGroup TranslationY => _TranslationY;
        public H3DFloatKeyFrameGroup TranslationZ => _TranslationZ;

        public bool ScaleExists       => _ScaleX.Exists       || _ScaleY.Exists       || _ScaleZ.Exists;

        public bool RotationExists    => _RotationX.Exists    || _RotationY.Exists    || _RotationZ.Exists;

        public bool TranslationExists => _TranslationX.Exists || _TranslationY.Exists || _TranslationZ.Exists;

        public H3DAnimTransform()
        {
            _ScaleX       = new H3DFloatKeyFrameGroup();
            _ScaleY       = new H3DFloatKeyFrameGroup();
            _ScaleZ       = new H3DFloatKeyFrameGroup();

            _RotationX    = new H3DFloatKeyFrameGroup();
            _RotationY    = new H3DFloatKeyFrameGroup();
            _RotationZ    = new H3DFloatKeyFrameGroup();

            _TranslationX = new H3DFloatKeyFrameGroup();
            _TranslationY = new H3DFloatKeyFrameGroup();
            _TranslationZ = new H3DFloatKeyFrameGroup();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            long Position = Deserializer.BaseStream.Position;

            uint ConstantMask = (uint)H3DAnimTransformFlags.IsScaleXConstant;
            uint NotExistMask = (uint)H3DAnimTransformFlags.IsScaleXInexistent;

            for (int ElemIndex = 0; ElemIndex < 9; ElemIndex++)
            {
                Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);

                Position += 4;

                bool Constant = ((uint)Flags & ConstantMask) != 0;
                bool Exists   = ((uint)Flags & NotExistMask) == 0;

                if (Exists)
                {
                    H3DFloatKeyFrameGroup FrameGrp = H3DFloatKeyFrameGroup.ReadGroup(Deserializer, Constant);

                    switch (ElemIndex)
                    {
                        case 0: _ScaleX       = FrameGrp; break;
                        case 1: _ScaleY       = FrameGrp; break;
                        case 2: _ScaleZ       = FrameGrp; break;

                        case 3: _RotationX    = FrameGrp; break;
                        case 4: _RotationY    = FrameGrp; break;
                        case 5: _RotationZ    = FrameGrp; break;

                        case 6: _TranslationX = FrameGrp; break;
                        case 7: _TranslationY = FrameGrp; break;
                        case 8: _TranslationZ = FrameGrp; break;
                    }
                }

                ConstantMask <<= 1;
                NotExistMask <<= 1;
                
                if (ConstantMask == (uint)H3DAnimTransformFlags.IsRotationWConstant)
                {
                    ConstantMask <<= 1;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            uint ConstantMask = (uint)H3DAnimTransformFlags.IsScaleXConstant;
            uint NotExistMask = (uint)H3DAnimTransformFlags.IsScaleXInexistent;

            long Position = Serializer.BaseStream.Position;

            Flags = 0;

            Serializer.Writer.Write(0u);

            for (int ElemIndex = 0; ElemIndex < 9; ElemIndex++)
            {
                H3DFloatKeyFrameGroup FrameGrp = null;

                switch (ElemIndex)
                {
                    case 0: FrameGrp = _ScaleX;       break;
                    case 1: FrameGrp = _ScaleY;       break;
                    case 2: FrameGrp = _ScaleZ;       break;

                    case 3: FrameGrp = _RotationX;    break;
                    case 4: FrameGrp = _RotationY;    break;
                    case 5: FrameGrp = _RotationZ;    break;

                    case 6: FrameGrp = _TranslationX; break;
                    case 7: FrameGrp = _TranslationY; break;
                    case 8: FrameGrp = _TranslationZ; break;
                }

                if (FrameGrp.KeyFrames.Count == 1)
                {
                    Flags |= (H3DAnimTransformFlags)ConstantMask;

                    Serializer.Writer.Write(FrameGrp.KeyFrames[0].Value);
                }
                else
                {
                    if (FrameGrp.KeyFrames.Count > 1)
                    {
                        Serializer.Sections[(uint)H3DSectionId.Contents].Values.Add(new RefValue()
                        {
                            Value    = FrameGrp,
                            Position = Serializer.BaseStream.Position
                        });
                    }
                    else
                    {
                        Flags |= (H3DAnimTransformFlags)NotExistMask;
                    }

                    Serializer.Writer.Write(0u);
                }

                ConstantMask <<= 1;
                NotExistMask <<= 1;
                
                if (ConstantMask == (uint)H3DAnimTransformFlags.IsRotationWConstant)
                {
                    ConstantMask <<= 1;
                }
            }

            Serializer.BaseStream.Seek(Position, SeekOrigin.Begin);

            Serializer.Writer.Write((uint)Flags);

            Serializer.BaseStream.Seek(Position + 4 + 9 * 4, SeekOrigin.Begin);

            return true;
        }
    }
}
