using SPICA.Serialization;

using System;
using System.IO;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimTransform : ICustomSerialization
    {
        private uint Flags;

        [NonSerialized] public H3DFloatKeyFrameGroup ScaleX;
        [NonSerialized] public H3DFloatKeyFrameGroup ScaleY;
        [NonSerialized] public H3DFloatKeyFrameGroup ScaleZ;

        [NonSerialized] public H3DFloatKeyFrameGroup RotationX;
        [NonSerialized] public H3DFloatKeyFrameGroup RotationY;
        [NonSerialized] public H3DFloatKeyFrameGroup RotationZ;

        [NonSerialized] public H3DFloatKeyFrameGroup TranslationX;
        [NonSerialized] public H3DFloatKeyFrameGroup TranslationY;
        [NonSerialized] public H3DFloatKeyFrameGroup TranslationZ;

        public H3DAnimTransform()
        {
            ScaleX       = new H3DFloatKeyFrameGroup();
            ScaleY       = new H3DFloatKeyFrameGroup();
            ScaleZ       = new H3DFloatKeyFrameGroup();

            RotationX    = new H3DFloatKeyFrameGroup();
            RotationY    = new H3DFloatKeyFrameGroup();
            RotationZ    = new H3DFloatKeyFrameGroup();

            TranslationX = new H3DFloatKeyFrameGroup();
            TranslationY = new H3DFloatKeyFrameGroup();
            TranslationZ = new H3DFloatKeyFrameGroup();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            long Position = Deserializer.BaseStream.Position;

            uint NotExistMask = 0x10000;
            uint ConstantMask = 0x40;

            for (int Elem = 0; Elem < 3; Elem++)
            {
                for (int Axis = 0; Axis < 3; Axis++)
                {
                    Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);

                    Position += 4;

                    bool Constant = (Flags & ConstantMask) != 0;
                    bool Exists = (Flags & NotExistMask) == 0;

                    if (Exists)
                    {
                        H3DFloatKeyFrameGroup FrameGrp = H3DFloatKeyFrameGroup.ReadGroup(Deserializer, Constant);

                        switch (Elem)
                        {
                            case 0:
                                switch (Axis)
                                {
                                    case 0: ScaleX = FrameGrp; break;
                                    case 1: ScaleY = FrameGrp; break;
                                    case 2: ScaleZ = FrameGrp; break;
                                }
                                break;
                            case 1:
                                switch (Axis)
                                {
                                    case 0: RotationX = FrameGrp; break;
                                    case 1: RotationY = FrameGrp; break;
                                    case 2: RotationZ = FrameGrp; break;
                                }
                                break;
                            case 2:
                                switch (Axis)
                                {
                                    case 0: TranslationX = FrameGrp; break;
                                    case 1: TranslationY = FrameGrp; break;
                                    case 2: TranslationZ = FrameGrp; break;
                                }
                                break;
                        }
                    }

                    NotExistMask <<= 1;
                    ConstantMask <<= 1;
                }

                if (Elem == 1) ConstantMask <<= 1; //Rotation W
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            throw new NotImplementedException();
        }
    }
}
